using Common;
using Common.IO;
using MaterialColor.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Common.Data;
using System.Linq;
using UnityEngine;
using MaterialColor.TemperatureOverlay;

namespace MaterialColor
{
    // TODO: move most of this stuff to Core
    public static class InjectionEntry
    {
        private static bool _initialized;

        private static bool _elementColorInfosChanged;
        private static bool _typeColorOffsetsChanged;
        private static bool _configuratorStateChanged;

        // TODO: merge with EnterEveryUpdate?
        public static void EnterOnce()
        {
            try
            {
                Components.BuildingCompletes.OnAdd += OnBuildingsCompletesAdd;

                if (!_initialized) Initialize();

                _elementColorInfosChanged = _typeColorOffsetsChanged = _configuratorStateChanged = true;
            }
            catch (Exception e)
            {
                var message = "Injection failed\n" + e.Message + '\n';

                State.Logger.Log(message);
                State.Logger.Log(e);

                Debug.LogError(message);
            }

            try
            {
                SaveTemperatureThresholdsAsDefault();

                if (State.TemperatureOverlayState.LogThresholds)
                {
                    LogTemperatureThresholds();
                }

                UpdateTemperatureThresholds();
            }
            catch (Exception e)
            {
                State.Logger.Log("Custom temperature overlay init error");
                State.Logger.Log(e);
            }
        }

        private static void UpdateTemperatureThresholds()
        {
            var newTemperatures = (State.TemperatureOverlayState.CustomRangesEnabled
                ? State.TemperatureOverlayState.Temperatures
                : State.DefaultTemperatures);

            for (int i = 0; i < newTemperatures.Count; i++)
            {
                SimDebugView.Instance.temperatureThresholds[i] = new SimDebugView.ColorThreshold { color = State.DefaultTemperatureColors[i], value = newTemperatures[i] };
            }

            Array.Sort(SimDebugView.Instance.temperatureThresholds, new ColorThresholdTemperatureSorter());
        }

        private static void LogTemperatureThresholds()
        {
            for (int i = 0; i < SimDebugView.Instance.temperatureThresholds.Length; i++)
            {
                State.Logger.Log(SimDebugView.Instance.temperatureThresholds[i].value.ToString());
                State.Logger.Log(SimDebugView.Instance.temperatureThresholds[i].color.ToString());
            }
        }

        private static void SaveTemperatureThresholdsAsDefault()
        {
            foreach (var threshold in SimDebugView.Instance.temperatureThresholds)
            {
                State.DefaultTemperatureColors.Add(threshold.color);
                State.DefaultTemperatures.Add(threshold.value);
            }
        }

        private static void Initialize()
        {
            SubscribeToFileChangeNotifier();
            _initialized = true;
        }

        public static void EnterEveryUpdate()
        {
            try
            {
                if (_elementColorInfosChanged || _typeColorOffsetsChanged || _configuratorStateChanged)
                {
                    RefreshMaterialColor();
                    _elementColorInfosChanged = _typeColorOffsetsChanged = _configuratorStateChanged = false;
                }
            }
            catch (Exception e)
            {
                State.Logger.Log("EnterEveryUpdate failed.");
                State.Logger.Log(e);
            }
        }

        public static Color EnterCell(Rendering.BlockTileRenderer blockRenderer, int cellIndex)
        {
            try
            {
                Color tileColor;

                if (State.ConfiguratorState.Enabled)
                {
                    if (State.ConfiguratorState.LegacyTileColorHandling)
                    {
                        switch (State.ConfiguratorState.ColorMode)
                        {
                            case Common.Data.ColorMode.Json:
                                tileColor = ColorHelper.GetCellColorJson(cellIndex);
                                break;
                            case Common.Data.ColorMode.DebugColor:
                                tileColor = ColorHelper.GetCellColorDebug(cellIndex);
                                break;
                            case ColorMode.None:
                            default:
                                tileColor = ColorHelper.DefaultCellColor;
                                break;
                        }
                    }
                    else
                    {
                        if (ColorHelper.TileColors.Length > cellIndex && ColorHelper.TileColors[cellIndex].HasValue)
                        {
                            tileColor = ColorHelper.TileColors[cellIndex].Value;
                        }
                        else
                        {
                            if (cellIndex == blockRenderer.invalidPlaceCell)
                                return ColorHelper.InvalidCellColor;

                            tileColor = ColorHelper.DefaultCellColor;
                        }
                    }
                }
                else
                {
                    tileColor = ColorHelper.DefaultCellColor;
                }

                if (cellIndex == blockRenderer.selectedCell)
                    return tileColor * 1.5f;

                if (cellIndex == blockRenderer.highlightCell)
                    return tileColor * 1.25f;

                return tileColor;
            }
            catch (Exception e)
            {
                State.Logger.Log("EnterCell failed.");
                State.Logger.Log(e);

                return ColorHelper.DefaultCellColor;
            }
        }

        private static bool _firstTimeEnumerate = true;

        private static void EnumerateOtherComponentsOnce(Component component)
        {
            if (!_firstTimeEnumerate) return;

            var comps = component.GetComponents<Component>();

            if (comps.Length <= 0) return;

            _firstTimeEnumerate = false;

            foreach (var comp in comps)
            {
                State.Logger.Log($"Component (BlockTileRenderer) Name/Type: {comp.name} / {comp.GetType()} ");
            }
        }

        public static bool EnterToggle(OverlayMenu overlayMenu, OverlayMenu.OverlayToggleInfo toggleInfo)
        {
            try
            {
                var toggleMaterialColor = toggleInfo.simView == (SimViewMode)Common.IDs.ToggleMaterialColorOverlayID;

                if (!toggleMaterialColor) return false;

                State.ConfiguratorState.Enabled = !State.ConfiguratorState.Enabled;

                RefreshMaterialColor();

                return true;
            }
            catch (Exception e)
            {
                State.Logger.Log("EnterToggle failed.");
                State.Logger.Log(e);
                return false;
            }
        }

        public static void RefreshMaterialColor()
        {
            UpdateBuildingsColors();
            RebuildAllTiles();
        }

        private static void RebuildAllTiles()
        {
            for (int i = 0; i < Grid.CellCount; i++)
            {
                World.Instance.blockTileRenderer.Rebuild(ObjectLayer.FoundationTile, i);
            }
            State.Logger.Log("All tiles rebuilt.");
        }

        public static void OverlayChangedEntry()
        {
            try
            {
                switch (OverlayScreen.Instance.GetMode())
                {
                    case SimViewMode.PowerMap:
                    case SimViewMode.GasVentMap:
                    case SimViewMode.LiquidVentMap:
                    case SimViewMode.Logic:
                        RefreshMaterialColor();
                        break;
                }
            }
            catch (Exception e)
            {
                State.Logger.Log("OverlayChangedEntry failed");
                State.Logger.Log(e);
            }
        }

        private static void SubscribeToFileChangeNotifier()
        {
            const string JSONFilter = "*.json";

            try
            {
                FileChangeNotifier.StartFileWatch(JSONFilter, Paths.ElementColorInfosDirectory, OnElementColorsInfosChanged);
                FileChangeNotifier.StartFileWatch(JSONFilter, Paths.TypeColorOffsetsDirectory, OnTypeColorOffsetsChanged);

                FileChangeNotifier.StartFileWatch(Paths.MaterialColorStateFileName, Paths.MaterialConfigPath, OnMaterialStateChanged);

                if (State.TemperatureOverlayState.CustomRangesEnabled)
                {
                    FileChangeNotifier.StartFileWatch(Paths.TemperatureStateFileName, Paths.OverlayConfigPath, OnTemperatureStateChanged);
                }
            }
            catch (Exception e)
            {
                State.Logger.Log("SubscribeToFileChangeNotifier failed");
                State.Logger.Log(e);
            }
        }

        private static void UpdateBuildingsColors()
        {
            State.Logger.Log($"Trying to update {Components.BuildingCompletes.Count} buildings.");

            try
            {
                foreach (var building in Components.BuildingCompletes)
                {
                    OnBuildingsCompletesAdd(building);
                }
                State.Logger.Log("Buildings updated successfully.");
            }
            catch (Exception e)
            {
                State.Logger.Log("Buildings colors update failed.");
                State.Logger.Log(e);
            }
        }

        // TODO: Move
        private static readonly Color _notGasColor = new Color(0.6f, 0.6f, 0.6f);

        // WIP
        // TODO: refactor
        public static Color EnterGasOverlay(int cellIndex)
        {
            var minMass = State.ConfiguratorState.GasPressureStart;
            var maxMass = State.ConfiguratorState.GasPressureEnd;

            var element = Grid.Element[cellIndex];

            if (!element.IsGas)
            {
                return _notGasColor;
            }

            Color gasColor = ColorHelper.GetCellOverlayColor(cellIndex);

            var gasMass = Grid.Cell[cellIndex].mass;

            gasMass -= minMass;

            if (gasMass < 0)
            {
                gasMass = 0;
            }

            maxMass -= minMass;

            if (maxMass < float.Epsilon)
            {
                maxMass = float.Epsilon;
            }

            var intensity = GetGasColorIntensity(gasMass, maxMass);

            gasColor *= intensity;
            gasColor.a = 1;

            return gasColor;
        }

        private static float GetGasColorIntensity(float mass, float maxMass)
        {
            var minIntensity = State.ConfiguratorState.MinimumGasColorIntensity;

            var intensity = mass / maxMass;

            intensity = Mathf.Sqrt(intensity);

            intensity = Mathf.Clamp01(intensity);
            intensity *= 1 - minIntensity;
            intensity += minIntensity;

            return intensity;
        }

        public static void ResetCell(int cellIndex)
        {
            if (ColorHelper.TileColors.Length > cellIndex)
            {
                ColorHelper.TileColors[cellIndex] = null;
            }
        }

        #region Event Handling

        private static void OnElementColorsInfosChanged(object sender, FileSystemEventArgs e)
        {
            bool reloadColorInfosResult = false;

            try
            {
                reloadColorInfosResult = State.TryReloadElementColorInfos();
            }
            catch (Exception ex)
            {
                State.Logger.Log("ReloadElementColorInfos failed.");
                State.Logger.Log(ex);
            }

            if (reloadColorInfosResult)
            {
                _elementColorInfosChanged = true;

                const string Message = "Element color infos changed.";

                State.Logger.Log(Message);
                Debug.LogError(Message);
            }
            else
            {
                State.Logger.Log("Reload element color infos failed");
            }
        }

        private static void OnTypeColorOffsetsChanged(object sender, FileSystemEventArgs e)
        {
            if (!State.TryReloadTypeColorOffsets()) return;

            _typeColorOffsetsChanged = true;

            const string Message = "Type colors changed.";

            State.Logger.Log(Message);
            Debug.LogError(Message);
        }

        private static void OnMaterialStateChanged(object sender, FileSystemEventArgs e)
        {
            if (!State.TryReloadConfiguratorState()) return;

            _configuratorStateChanged = true;

            const string Message = "Configurator state changed.";

            State.Logger.Log(Message);
            Debug.LogError(Message);
        }

        // TODO: log failed reload on other eventhandlers
        private static void OnTemperatureStateChanged(object sender, FileSystemEventArgs e)
        {
            string message;

            if (State.TryReloadTemperatureState())
            {
                UpdateTemperatureThresholds();
                message = "Temperature overlay state changed.";
            }
            else
            {
                message = "Temperature overlay state load failed.";
            }

            State.Logger.Log(message);
            Debug.LogError(message);
        }

        private static void OnBuildingsCompletesAdd(BuildingComplete building)
            => ColorHelper.UpdateBuildingColor(building);

        #endregion
    }
}
