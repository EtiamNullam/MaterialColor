using MaterialColor.Common.Json;
using MaterialColor.Core.Extensions;
using MaterialColor.Core.Helpers;
using MaterialColor.Core.IO;
using System;
using System.IO;
using UnityEngine;

namespace MaterialColor.Core
{
    // TODO: refactor
    public static class InjectionEntry
    {
        private static bool Initialized = false;

        private static bool ElementColorInfosChanged = false;
        private static bool TypeColorOffsetsChanged = false;
        private static bool ConfiguratorStateChanged = false;

        private static JsonFileLoader _jsonLoader = new JsonFileLoader(new JsonManager());
        private static FileChangeNotifier _fileChangeNotifier = new FileChangeNotifier();

        public static void EnterOnce()
        {
            try
            {
                Components.BuildingCompletes.OnAdd += OnBuildingsCompletesAdd;

                _jsonLoader.ReloadAll();

                if (!Initialized) Initialize();
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
            OverlayScreen.OnOverlayChanged += OnOverlayChanged;

            StartFileChangeNotifier();
            Initialized = true;
        }

        public static void EnterEveryUpdate()
        {
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

        private static void StartFileChangeNotifier()
        {
            _fileChangeNotifier.ElementColorInfosChanged += OnElementColorsInfosChanged;
            _fileChangeNotifier.TypeColorOffsetsChanged += OnTypeColorOffsetsChanged;
            _fileChangeNotifier.ConfiguratorStateChanged += OnConfiguratorStateChanged;
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
                Debug.LogError("Element color infos changed.");
                ElementColorInfosChanged = true;
            }
        }

        private static void OnTypeColorOffsetsChanged(object sender, FileSystemEventArgs e)
        {
            if (_jsonLoader.TryLoadTypeColorOffsets())
            {
                Debug.LogError("Type colors changed.");
                TypeColorOffsetsChanged = true;
            }
        }

        private static void OnConfiguratorStateChanged(object sender, FileSystemEventArgs e)
        {
            if (_jsonLoader.TryLoadConfiguratorState())
            {
                Debug.LogError("Configurator state changed.");
                ConfiguratorStateChanged = true;
            }
        }

        private static void OnBuildingsCompletesAdd(BuildingComplete building)
            => ColorHelper.UpdateBuildingColor(building);
    }
}
