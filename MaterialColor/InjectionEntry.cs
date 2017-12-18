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
        }

        public static void EnterEveryUpdate()
        {
            try
            {
                if (ElementColorInfosChanged || TypeColorOffsetsChanged || ConfiguratorStateChanged)
                {
                    RefreshMaterialColor();
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
                            {
                                return ColorHelper.InvalidCellColor;
                            }
                            else
                            {
                                tileColor = ColorHelper.DefaultCellColor;
                            }
                        }
                    }
                }
                else
                {
                    tileColor = ColorHelper.DefaultCellColor;
                }

                if (cellIndex == blockRenderer.selectedCell)
                {
                    return tileColor * 1.5f;
                }
                else if (cellIndex == blockRenderer.highlightCell)
                {
                    return tileColor * 1.25f;
                }
                else
                {
                    return tileColor;
                }
            }
            catch (Exception e)
            {
                State.Logger.Log("EnterCell failed.");
                State.Logger.Log(e);

                return ColorHelper.DefaultCellColor;
            }
        }

        private static bool firstTimeEnumerate = true;

        private static void EnumerateOtherComponentsOnce(Component component)
        {
            if (firstTimeEnumerate)
            {
                var comps = component.GetComponents<Component>();

                if (comps.Length > 0)
                {
                    firstTimeEnumerate = false;

                    foreach (var comp in comps)
                    {
                        State.Logger.Log($"Component (BlockTileRenderer) Name/Type: {comp.name} / {comp.GetType()} ");
                    }
                }
            }
        }

        public static bool EnterToggle(OverlayMenu overlayMenu, OverlayMenu.OverlayToggleInfo toggleInfo)
        {
            try
            {
                var toggleMaterialColor = toggleInfo.simView == (SimViewMode)Common.IDs.ToggleMaterialColorOverlayID;

                if (toggleMaterialColor)
                {
                    State.ConfiguratorState.Enabled = !State.ConfiguratorState.Enabled;

                    RefreshMaterialColor();
                }

                return toggleMaterialColor;
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
                if (OverlayScreen.Instance.GetMode() != SimViewMode.None)
                {
                    RefreshMaterialColor();
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
