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

        private static JsonManager _jsonManager = new JsonManager();
        private static ConfiguratorStateManager _configuratorStateManager = new ConfiguratorStateManager(_jsonManager);
        private static ElementColorInfosManager _elementColorInfosManager = new ElementColorInfosManager(_jsonManager);
        private static TypeColorOffsetsManager _typeColorOffsetsManager = new TypeColorOffsetsManager(_jsonManager);

        private static FileChangeNotifier _fileChangeNotifier = new FileChangeNotifier();

        public static void EnterOnce()
        {
            //if (State.ConfiguratorState.ShowDetailedErrorInfo)
            //{
            //Debug.LogError("Enter Once");
            //return;
            //}

            try
            {
                Components.BuildingCompletes.OnAdd += OnBuildingsCompletesAdd;

                ReloadAll();

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

        private static void ReloadAll()
        {
            TryLoadElementColorInfos();
            TryLoadTypeColorOffsets();
            TryLoadConfiguratorState();
        }

        private static void Initialize()
        {
            OverlayScreen.OnOverlayChanged += OnOverlayChanged;

            StartFileChangeNotifier();
            Initialized = true;
        }

        public static void EnterEveryUpdate()
        {
            var changed = false;

            if (ElementColorInfosChanged)
            {
                TryLoadElementColorInfos();
                changed = true;
            }

            if (TypeColorOffsetsChanged)
            {
                TryLoadTypeColorOffsets();
                changed = true;
            }

            if (changed)
            {
                UpdateBuildingsColors();
                ElementColorInfosChanged = TypeColorOffsetsChanged = false;
            }
        }

        // TODO: refactor
        public static Color EnterCell(Rendering.BlockTileRenderer blockRenderer, int cellIndex)
        {
            Color materialColor;

            if (!State.Disabled)
            {
                switch (State.ConfiguratorState.ColorMode)
                {
                    case Common.Data.ColorMode.Json:
                        var material = GetMaterialFromCell(cellIndex);
                        materialColor = material.ToCellMaterialColor();
                        break;
                    case Common.Data.ColorMode.DebugColor:
                        materialColor = ElementLoader.elements[Grid.Cell[cellIndex].elementIdx].substance.debugColour;
                        materialColor.a = 1;
                        break;
                    default:
                        materialColor = new Color(1, 1, 1);
                        break;
                }
            }
            else materialColor = new Color(1, 1, 1);

            return blockRenderer.highlightCell == cellIndex
                ? materialColor * 1.25f
                : blockRenderer.selectedCell == cellIndex
                    ? materialColor * 1.5f
                    : materialColor;
        }

        // TODO: refactor
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

        public static void SetLocalizationString()
        {
            STRINGS.INPUT_BINDINGS.ROOT.OVERLAY12 = "Toggle_MaterialColor_Overlay";
        }

        private static SimHashes GetMaterialFromCell(int cellIndex)
        {
            if (!Grid.IsValidCell(cellIndex))
            {
                return SimHashes.Vacuum;
            }

            var cellElementIndex = Grid.Cell[cellIndex].elementIdx;
            var element = ElementLoader.elements[cellElementIndex];

            return element.id;
        }

        private static void OnOverlayChanged(SimViewMode obj)
        {
            Debug.LogError(obj.ToString());

            if (!State.Disabled && obj == SimViewMode.None)
            {
                UpdateBuildingsColors();
            }
        }

        // convert to instance class and dispose properly of events and FileChangeNotifier
        private static void StartFileChangeNotifier()
        {
            _fileChangeNotifier.ElementColorInfosChanged += OnElementColorsInfosChanged;
            _fileChangeNotifier.TypeColorOffsetsChanged += OnTypeColorOffsetsChanged;
            _fileChangeNotifier.ConfiguratorStateChanged += OnConfiguratorStateChanged;
        }

        private static void OnConfiguratorStateChanged(object sender, FileSystemEventArgs e)
        {
            TryLoadConfiguratorState();
        }

        private static void TryLoadConfiguratorState()
        {
            try
            {
                State.ConfiguratorState = _configuratorStateManager.LoadState();
            }
            catch (Exception ex)
            {
                var message = "Can't load configurator state.\n" + ex.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + ex.StackTrace;
                }
                Debug.LogError(message);

                return;
            }
        }

        private static void TryLoadElementColorInfos()
        {
            try
            {
                State.ElementColorInfos = _elementColorInfosManager.LoadElementColorInfos();
            }
            catch (Exception e)
            {
                var message = "Can't load ElementColorInfos\n" + e.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + e.StackTrace;
                }
                Debug.LogError(message);
            }
        }

        private static void TryLoadTypeColorOffsets()
        {
            try
            {
                State.TypeColorOffsets = _typeColorOffsetsManager.LoadTypeColorOffsets();
            }
            catch (Exception e)
            {
                var message = "Can't load TypeColorOffsets\n" + e.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + e.StackTrace;
                }

                Debug.LogError(message);
            }
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
            Debug.LogError("Element color infos changed.");

            ElementColorInfosChanged = true;
        }

        private static void OnTypeColorOffsetsChanged(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("Type colors changed.");

            TypeColorOffsetsChanged = true;
        }

        private static void OnBuildingsCompletesAdd(BuildingComplete building)
            => ColorHelper.UpdateBuildingColor(building);
    }
}
