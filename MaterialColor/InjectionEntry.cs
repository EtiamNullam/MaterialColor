using Common;
using Common.IO;
using Common.Json;
using MaterialColor.Extensions;
using MaterialColor.Helpers;
using MaterialColor.IO;
using System;
using System.IO;
using UnityEngine;

namespace MaterialColor
{
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

        public static Color EnterCell(Rendering.BlockTileRenderer blockRenderer, int cellIndex)
        {
            Color resultColor;

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

        public static bool EnterToggle(OverlayMenu overlayMenu, KIconToggleMenu.ToggleInfo toggleInfo)
        {
            if (toggleInfo.userData is SimViewMode userDataAsSimViewMode
                && userDataAsSimViewMode == (SimViewMode)Common.IDs.ToggleMaterialColorOverlayID)
            {
                State.ConfiguratorState.Enabled = !State.ConfiguratorState.Enabled;
                UpdateBuildingsColors();
                RebuildAllTiles();

                return true;
            }
            else return false;
        }

        private static void RebuildAllTiles()
        {
            for (int i = 0; i < Grid.CellCount; i++)
            {
                World.Instance.blockTileRenderer.Rebuild(ObjectLayer.FoundationTile, i);
            }
        }

        // merge with EnterOnce?
        public static void SetLocalizationString()
        {
            STRINGS.INPUT_BINDINGS.ROOT.OVERLAY12 = "Toggle_MaterialColor_Overlay";
        }

        private static void OnOverlayChanged(SimViewMode obj)
        {
            if (State.ConfiguratorState.Enabled && obj == SimViewMode.None)
            {
                UpdateBuildingsColors();
            }
        }

        private static void SubscribeToFileChangeNotifier()
        {
            var jsonFilter = "*.json";

            FileChangeNotifier.StartFileWatch(jsonFilter, Paths.ElementColorInfosDirectory, OnElementColorsInfosChanged);
            FileChangeNotifier.StartFileWatch(jsonFilter, Paths.TypeColorOffsetsDirectory, OnTypeColorOffsetsChanged);

            FileChangeNotifier.StartFileWatch(Paths.MaterialColorStateFileName, Paths.MaterialConfigPath, OnMaterialStateChanged);
        }

        private static void UpdateBuildingsColors()
        {
            foreach (var building in Components.BuildingCompletes)
            {
                OnBuildingsCompletesAdd(building);
            }
        }

        private static void OnElementColorsInfosChanged(object sender, FileSystemEventArgs e)
        {
            if (State.TryReloadElementColorInfos())
            {
                ElementColorInfosChanged = true;

                var message = "Element color infos changed.";

                State.Logger.Log(message);
                Debug.LogError(message);
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
    }
}
