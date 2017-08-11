using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MaterialColor
{
    public static class InjectionEntry
    {
        public static void EnterOnce()
        {
            Debug.LogError("Enter Once");
            //return;
            try
            {
                TryLoadElementColorInfos();
                TryLoadTypeColorOffsets();

                // unsubscribe?
                Components.BuildingCompletes.OnAdd += OnBuildingsCompletesAdd;

                ElementColorInfosChanged = TypeColorOffsetsChanged = true;

                //TryLoadInjectorState();
                TryLoadConfiguratorState();

                OverlayScreen.OnOverlayChanged += OnOverlayChanged;

                StartFileChangeNotifier();
            }
            catch (Exception e)
            {
                Debug.LogError("Injection failed\n"
                    + e.Message + '\n'
                    + e.StackTrace
                    );
            }
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
        public static UnityEngine.Color EnterCell(Rendering.BlockTileRenderer blockRenderer, int cellIndex)
        {
            // doesnt work for gas permeable tiles (shows gas element)
            // changed color of white blueprint when building new tiles
            var cellElementIndex = Grid.Cell[cellIndex].elementIdx;

            // test

            //try
            //{
            //    var cellOnTile = Grid.Objects[cellIndex, (int)Grid.SceneLayer.TileMain];

            //    if (cellOnTile != null)
            //    {
            //        Debug.LogError(cellOnTile.GetType().ToString());
            //    }
            //}
            //catch (Exception e)
            //{
            //    Debug.LogError("Can't get cell " + cellIndex + '\n' + e.Message);
            //}

            // grid elements indexer test
            // WIP

            //var cellOnTile = Grid.

            // temporarily disabled
            var elementHash = ElementLoader.elements[cellElementIndex].id;

            var materialColor = MaterialColorGuard.GetCellMaterialColor(elementHash);

            if (blockRenderer.highlightCell == cellIndex)
            {
                return materialColor * 1.25f;
            }
            else if (blockRenderer.selectedCell == cellIndex)
            {
                return materialColor * 1.5f;
            }
            return materialColor;

            // old handling

            //if (blockRenderer.highlightCell == cellIndex)
            //{
            //    return new UnityEngine.Color(1, 1, 1) * 1.5f;
            //}
            //else if (blockRenderer.selectedCell == cellIndex)
            //{
            //    return new UnityEngine.Color(1, 1, 1) * 1.25f;
            //}
            //else return new UnityEngine.Color(1, 1, 1);
            //

        }

        private static void OnOverlayChanged(SimViewMode obj)
        {
            //Debug.LogError(obj.ToString());
            if (obj == SimViewMode.None)
            {
                UpdateBuildingsColors();
            }
        }

        private static void StartFileChangeNotifier()
        {
            // convert to instance class and dispose properly of events and FileChangeNotifier
            _fileChangeNotifier = _fileChangeNotifier ?? new FileChangeNotifier();

            _fileChangeNotifier.ElementColorInfosChanged += OnElementColorsInfosUpdated;
            _fileChangeNotifier.TypeColorsChanged += OnTypeColorOffsetsUpdated;
            _fileChangeNotifier.ConfiguratorStateChanged += OnConfiguratorStateChanged;
        }

        //
        //
        // load configurator state instead
        //
        //
        private static void OnConfiguratorStateChanged(object sender, FileSystemEventArgs e)
        {
            //TryLoadInjectorState();
            TryLoadConfiguratorState();
        }

        private static void TryLoadConfiguratorState()
        {
            var configuratorStateManager = new Common.Json.ConfiguratorStateManager();

            try
            {
                MaterialColorGuard.ConfiguratorState = configuratorStateManager.LoadState();
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
                MaterialColorGuard.ElementColorInfos = elementColorInfosJsonManager.LoadElementColorInfos();
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
                MaterialColorGuard.TypeColorOffsets = typeColorOffsetsJsonManager.LoadTypeColorOffsets();
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
        //private static bool SkipTiles = false;

        private static void UpdateBuildingsColors()
        {
            foreach (var building in Components.BuildingCompletes)
            {
                OnBuildingsCompletesAdd(building);
            }
        }

        private static void OnElementColorsInfosUpdated(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("Element color infos changed");

            ElementColorInfosChanged = true;
        }

        private static void OnTypeColorOffsetsUpdated(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("Type colors changed");

            TypeColorOffsetsChanged = true;
        }

        private static FileChangeNotifier _fileChangeNotifier;

        // refrigerator, storagelocker, bed, medicalcot and others needs special case
        // (ownable class)
        // tile doesnt have KAnimControllerBase, find another way to tint it
        // TODO: refactor
        private static void OnBuildingsCompletesAdd(BuildingComplete building)
        {
            UpdateBuildingColor(building);
        }

        private static void UpdateBuildingColor(BuildingComplete building)
        {
            var buildingName = building.name.Replace("Complete", string.Empty);
            var kAnimControllerBase = building.GetComponent<KAnimControllerBase>();

            if (kAnimControllerBase != null)
            {

                var color = MaterialColorGuard.GetMaterialColor(kAnimControllerBase, buildingName);

                kAnimControllerBase.TintColour = color;

                var dimmedColor = color.SetBrightness(color.GetBrightness() / 2);

                // storagelocker
                var storageLocker = building.GetComponent<StorageLocker>();

                if (storageLocker != null)
                {
                    storageLocker.filterTint = color;
                    storageLocker.noFilterTint = dimmedColor;
                }
                else // ownable
                {
                    var ownable = building.GetComponent<Ownable>();

                    if (ownable != null)
                    {
                        ownable.ownedTint = color;
                        ownable.unownedTint = dimmedColor;
                    }
                    else // rationbox
                    {
                        var rationBox = building.GetComponent<RationBox>();

                        if (rationBox != null)
                        {
                            rationBox.filterTint = color;
                            rationBox.noFilterTint = dimmedColor;
                        }
                    }
                }
            }
            else if (!TileNames.Contains(buildingName))
            {
                Debug.LogError($"Can't find KAnimControllerBase component in {buildingName}");
            }
        }

        private static List<string> TileNames = new List<string>
        {
            "Tile", "MeshTile", "InsulationTile", "GasPermeableMembrane"
        };
    }
}
