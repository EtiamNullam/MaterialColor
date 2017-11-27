using Common;
using Common.IO;
using MaterialColor.Helpers;
using System;
using System.IO;
using UnityEngine;

namespace MaterialColor
{
    // TODO: move most of this stuff to Core
    public static class InjectionEntry
    {
        private static bool Initialized = false;

        private static bool ElementColorInfosChanged = false;
        private static bool TypeColorOffsetsChanged = false;
        private static bool ConfiguratorStateChanged = false;

        private static bool _firstUpdate = false;

        // TODO: merge with EnterEveryUpdate?
        public static void EnterOnce()
        {
            try
            {
                Components.BuildingCompletes.OnAdd += OnBuildingsCompletesAdd;

                if (!Initialized) Initialize();

                ElementColorInfosChanged = TypeColorOffsetsChanged = ConfiguratorStateChanged = true;
            }
            catch (Exception e)
            {
                var message = "Injection failed\n" + e.Message + '\n';

                State.Logger.Log(message);
                State.Logger.Log(e);

                Debug.LogError(message);
            }
        }

        private static void Initialize()
        {
            SubscribeToFileChangeNotifier();
            Initialized = true;
            _firstUpdate = true;
        }

        public static void EnterEveryUpdate()
        {
            try
            {
                if (_firstUpdate)
                {
                    if (OverlayScreen.Instance != null)
                    {
                        OverlayScreen.Instance.OnOverlayChanged += OnOverlayChanged;
                        _firstUpdate = false;
                    }
                    else State.Logger.Log("OverlayScreen.Instance is null");
                }

                if (ElementColorInfosChanged || TypeColorOffsetsChanged || ConfiguratorStateChanged)
                {
                    UpdateBuildingsColors();
                    RebuildAllTiles();
                    ElementColorInfosChanged = TypeColorOffsetsChanged = ConfiguratorStateChanged = false;
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
            Color resultColor;

            try
            {
                if (State.ConfiguratorState.Enabled)
                {
                    switch (State.ConfiguratorState.ColorMode)
                    {
                        case Common.Data.ColorMode.Json:
                            resultColor = ColorHelper.GetCellColorJson(cellIndex);
                            break;
                        case Common.Data.ColorMode.DebugColor:
                            resultColor = ColorHelper.GetCellColorDebug(cellIndex);
                            break;
                        default:
                            resultColor = ColorHelper.GetDefaultCellColor();
                            break;
                    }
                }
                else resultColor = ColorHelper.GetDefaultCellColor();

                return blockRenderer.highlightCell == cellIndex
                    ? resultColor * 1.25f
                    : blockRenderer.selectedCell == cellIndex
                        ? resultColor * 1.5f
                        : resultColor;
            }
            catch (Exception e)
            {
                State.Logger.Log("EnterCell failed.");
                State.Logger.Log(e);

                return ColorHelper.GetDefaultCellColor();
            }
        }

        public static bool EnterToggle(OverlayMenu overlayMenu, OverlayMenu.OverlayToggleInfo toggleInfo)
        {
            try
            {
                if (toggleInfo.simView == (SimViewMode)Common.IDs.ToggleMaterialColorOverlayID)
                {
                    State.ConfiguratorState.Enabled = !State.ConfiguratorState.Enabled;
                    UpdateBuildingsColors();
                    RebuildAllTiles();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                State.Logger.Log("EnterToggle failed.");
                State.Logger.Log(e);
                return false;
            }
        }

        private static void RebuildAllTiles()
        {
            for (int i = 0; i < Grid.CellCount; i++)
            {
                World.Instance.blockTileRenderer.Rebuild(ObjectLayer.FoundationTile, i);
            }
            State.Logger.Log("All tiles rebuilt.");
        }

        private static void OnOverlayChanged(SimViewMode obj)
        {
            try
            {
                if (State.ConfiguratorState.Enabled && obj == SimViewMode.None)
                {
                    UpdateBuildingsColors();
                }

            }
            catch(Exception e)
            {
                State.Logger.Log("OnOverlayChangedFailed");
                State.Logger.Log(e);
            }
        }

        private static void SubscribeToFileChangeNotifier()
        {
            var jsonFilter = "*.json";

            try
            {
                FileChangeNotifier.StartFileWatch(jsonFilter, Paths.ElementColorInfosDirectory, OnElementColorsInfosChanged);
                FileChangeNotifier.StartFileWatch(jsonFilter, Paths.TypeColorOffsetsDirectory, OnTypeColorOffsetsChanged);

                FileChangeNotifier.StartFileWatch(Paths.MaterialColorStateFileName, Paths.MaterialConfigPath, OnMaterialStateChanged);
            }
            catch (Exception e)
            {
                State.Logger.Log("SubscribeToFIleChangeNotifierFailed");
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
                State.Logger.Log($"Buildings updated successfully.");
            }
            catch (Exception e)
            {
                State.Logger.Log("Buildings colors update failed.");
                State.Logger.Log(e);
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
                ElementColorInfosChanged = true;

                var message = "Element color infos changed.";

                State.Logger.Log(message);
                Debug.LogError(message);
            }
            else
            {
                State.Logger.Log("Reload element color infos failed");
            }
        }

        private static void OnTypeColorOffsetsChanged(object sender, FileSystemEventArgs e)
        {
            if (State.TryReloadTypeColorOffsets())
            {
                TypeColorOffsetsChanged = true;

                var message = "Type colors changed.";

                State.Logger.Log(message);
                Debug.LogError(message);
            }
        }

        private static void OnMaterialStateChanged(object sender, FileSystemEventArgs e)
        {
            if (State.TryReloadConfiguratorState())
            {
                ConfiguratorStateChanged = true;

                var message = "Configurator state changed.";

                State.Logger.Log(message);
                Debug.LogError(message);
            }
        }

        private static void OnBuildingsCompletesAdd(BuildingComplete building)
            => ColorHelper.UpdateBuildingColor(building);

        #endregion
    }
}
