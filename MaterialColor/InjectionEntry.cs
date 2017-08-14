using MaterialColor.Extensions;
using MaterialColor.Helpers;
using MaterialColor.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MaterialColor
{
    public static class InjectionEntry
    {
        private static bool Initialized = false;

        public static void EnterOnce()
        {
            Debug.LogError("Enter Once");

            try
            {
                if (!Initialized) Initialize();

                ReloadAll();
            }
            catch (Exception e)
            {
                Debug.LogError("Injection failed\n"
                    + e.Message + '\n'
                    + e.StackTrace
                    );
            }
        }

        private static void Initialize()
        {
            Components.BuildingCompletes.OnAdd += OnBuildingsCompletesAdd;

            // unsubscribe?
            ElementColorInfosChanged = TypeColorOffsetsChanged = true;

            OverlayScreen.OnOverlayChanged += OnOverlayChanged;

            StartFileChangeNotifier();
            Initialized = true;
        }

        private static void ReloadAll()
        {
            TryLoadElementColorInfos();
            TryLoadTypeColorOffsets();
            TryLoadConfiguratorState();
        }

        public static void EnterEveryUpdate()
        {
            var changed = false;

            if (ElementColorInfosChanged)
            {
                TryLoadElementColorInfos();
                changed = true;

                //temp
                TryLoadTypeColorOffsets();
            }

            if (TypeColorOffsetsChanged)
            {
                TryLoadTypeColorOffsets();
                changed = true;

                //temp
                TryLoadElementColorInfos();
            }

            if (changed)
            {
                UpdateBuildingsColors();
                ElementColorInfosChanged = TypeColorOffsetsChanged = false;
            }
        }

        // TODO: find a way to show mesh and gas permeable tiles' material color
        // maybe not possible?
        public static UnityEngine.Color EnterCell(Rendering.BlockTileRenderer blockRenderer, int cellIndex)
        {
            // doesnt work for gas permeable tiles (shows gas element)
            // changed color of white blueprint when building new tiles

            var material = GetMaterialFromCell(cellIndex);
            var materialColor = material.ToCellMaterialColor();

            if (blockRenderer.highlightCell == cellIndex)
            {
                return materialColor * 1.25f;
            }
            else if (blockRenderer.selectedCell == cellIndex)
            {
                return materialColor * 1.5f;
            }
            return materialColor;
        }

        private static SimHashes GetMaterialFromCell(int cellIndex)
        {
            var cellElementIndex = Grid.Cell[cellIndex].elementIdx;
            var element = ElementLoader.elements[cellElementIndex];

            return element.id;
        }

        private static void OnOverlayChanged(SimViewMode obj)
        {
            if (obj == SimViewMode.None)
            {
                UpdateBuildingsColors();
            }
        }

        private static void StartFileChangeNotifier()
        {
            if (_fileChangeNotifier == null)
            {
                //_fileChangeNotifier = new FileChangeNotifier();
                _fileChangeNotifier = _fileChangeNotifier ?? new FileChangeNotifier();
                _fileChangeNotifier.ElementColorInfosChanged += OnElementColorsInfosUpdated;
                _fileChangeNotifier.TypeColorsChanged += OnTypeColorOffsetsUpdated;
                _fileChangeNotifier.ConfiguratorStateChanged += OnConfiguratorStateChanged;
            }
            // convert to instance class and dispose properly of events and FileChangeNotifier
        }

        private static void OnConfiguratorStateChanged(object sender, FileSystemEventArgs e)
        {
            TryLoadConfiguratorState();
        }

        private static void TryLoadConfiguratorState()
        {
            var configuratorStateManager = new Common.Json.ConfiguratorStateManager();

            try
            {
                State.ConfiguratorState = configuratorStateManager.LoadState();
            }
            catch (Exception ex)
            {
                Debug.LogError("Can't load configurator state.\n"
                    + ex.Message + '\n'
                    + ex.StackTrace
                    );

                return;
            }
        }

        private static void TryLoadElementColorInfos()
        {
            var elementColorInfosJsonManager = new Common.Json.ElementColorInfosManager();

            try
            {
                State.ElementColorInfos = elementColorInfosJsonManager.LoadElementColorInfos();
            }
            catch (Exception e)
            {
                Debug.LogError("Can't load ElementColorInfos\n"
                    + e.Message + '\n'
                    + e.StackTrace
                    );
            }
        }

        private static void TryLoadTypeColorOffsets()
        {
            var typeColorOffsetsJsonManager = new Common.Json.TypeColorOffsetsManager();

            try
            {
                State.TypeColorOffsets = typeColorOffsetsJsonManager.LoadTypeColorOffsets();
            }
            catch (Exception e)
            {
                Debug.LogError("Can't load TypeColorOffsets\n"
                    + e.Message + '\n'
                    + e.StackTrace
                    );
            }
        }

        // doesnt work after 2nd world load
        private static bool ElementColorInfosChanged = false;
        private static bool TypeColorOffsetsChanged = false;

        private static void UpdateBuildingsColors()
        {
            foreach (var building in Components.BuildingCompletes)
            {
                OnBuildingsCompletesAdd(building);
            }
        }

        private static void OnElementColorsInfosUpdated(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("Element color infos changed.");

            ElementColorInfosChanged = true;
        }

        private static void OnTypeColorOffsetsUpdated(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("Type colors changed.");

            TypeColorOffsetsChanged = true;
        }

        private static FileChangeNotifier _fileChangeNotifier;

        private static void OnBuildingsCompletesAdd(BuildingComplete building)
            => ColorHelper.UpdateBuildingColor(building);

    }
}
