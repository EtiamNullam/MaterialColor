using MaterialColor.Common.IO;
using MaterialColor.Common.Json;
using MaterialColor.Core.Extensions;
using MaterialColor.Core.Helpers;
using MaterialColor.Core.IO;
using System;
using System.IO;
using UnityEngine;

namespace MaterialColor.Core
{
    public static class InjectionEntry
    {
        private static bool Initialized = false;

        private static bool ElementColorInfosChanged = false;
        private static bool TypeColorOffsetsChanged = false;
        private static bool ConfiguratorStateChanged = false;

        private static JsonFileLoader _jsonLoader = new JsonFileLoader(new JsonManager());

        private static bool _firstUpdate = false;

        // TODO: merge with EnterEveryUpdate?
        public static void EnterOnce()
        {
            try
            {
                Components.BuildingCompletes.OnAdd += OnBuildingsCompletesAdd;

                _jsonLoader.ReloadAll();

                if (!Initialized) Initialize();

                ElementColorInfosChanged = TypeColorOffsetsChanged = ConfiguratorStateChanged = true;
            }
            catch (Exception e)
            {
                var message = "Injection failed\n" + e.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + e.StackTrace;
                }

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
                else if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    Debug.LogError("OverlayScreen.Instance is null");
                }
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

            if (!State.Disabled)
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
                State.Disabled = !State.Disabled;
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
            if (!State.Disabled && obj == SimViewMode.None)
            {
                UpdateBuildingsColors();
            }
        }

        private static void SubscribeToFileChangeNotifier()
        {
            FileChangeNotifier.StartFileWatch(Common.Paths.ElementColorInfosFilePath, Common.Paths.ConfigDirectory, OnElementColorsInfosChanged);
            FileChangeNotifier.StartFileWatch(Common.Paths.TypeColorsFilePath, Common.Paths.ConfigDirectory, OnTypeColorOffsetsChanged);
            FileChangeNotifier.StartFileWatch(Common.Paths.MaterialColorStateFilePath, Common.Paths.ConfigDirectory, OnMaterialStateChanged);
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
            if (_jsonLoader.TryLoadElementColorInfos())
            {
                ElementColorInfosChanged = true;
                Debug.LogError("Element color infos changed.");
            }
        }

        private static void OnTypeColorOffsetsChanged(object sender, FileSystemEventArgs e)
        {
            if (_jsonLoader.TryLoadTypeColorOffsets())
            {
                TypeColorOffsetsChanged = true;
                Debug.LogError("Type colors changed.");
            }
        }

        private static void OnMaterialStateChanged(object sender, FileSystemEventArgs e)
        {
            if (_jsonLoader.TryLoadConfiguratorState())
            {
                ConfiguratorStateChanged = true;
                Debug.LogError("Configurator state changed.");
            }
        }

        private static void OnBuildingsCompletesAdd(BuildingComplete building)
            => ColorHelper.UpdateBuildingColor(building);
    }
}
