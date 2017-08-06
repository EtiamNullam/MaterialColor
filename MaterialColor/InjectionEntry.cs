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
            //Debug.LogError("Injected");
            //return;
            try
            {
                TryLoadElementColorInfos();
                TryLoadTypeColorOffsets();

                // unsubscribe?
                Components.BuildingCompletes.OnAdd += OnBuildingsCompletesAdd;

                ElementColorInfosChanged = TypeColorOffsetsChanged = true;

                TryLoadInjectorState();

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

        private static void StartFileChangeNotifier()
        {
            // convert to instance class and dispose properly of events and FileChangeNotifier
            _fileChangeNotifier = _fileChangeNotifier ?? new FileChangeNotifier();

            _fileChangeNotifier.ElementColorInfosChanged += OnElementColorsInfosUpdated;
            _fileChangeNotifier.TypeColorsChanged += OnTypeColorOffsetsUpdated;
            _fileChangeNotifier.InjectorStateChanged += OnInjectorStateChanged;
        }

        private static void OnInjectorStateChanged(object sender, FileSystemEventArgs e)
        {
            TryLoadInjectorState();
        }

        private static void TryLoadInjectorState()
        {
            var injectorStateManager = new Common.Json.InjectorStateManager();

            List<bool> state;

            try
            {
                state = injectorStateManager.LoadState();
            }
            catch (Exception ex)
            {
                Debug.LogError("Can't load injector state\n"
                    + ex.Message + '\n'
                    + ex.StackTrace
                    );

                return;
            }

            if (state.Count >= 5)
            {
                MaterialColorGuard.ShowMissingElements = state[1];
                MaterialColorGuard.ShowMissingTypes = state[2];
                SkipTiles = state[3];
                MaterialColorGuard.SetColorableObjectsAsWhite = state[4];
            }
            else
            {
                Debug.LogError("Invalid injector state.");
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
                Debug.LogError("Can't load TypeColorOffests\n"
                    + e.Message + '\n'
                    + e.StackTrace
                    );
            }
        }

        public static void EnterEveryUpdate()
        {
            if (ElementColorInfosChanged || TypeColorOffsetsChanged)
            {
                TryLoadElementColorInfos();
                TryLoadTypeColorOffsets();

                UpdateBuildingsColors();

                ElementColorInfosChanged = TypeColorOffsetsChanged = false;
            }
        }

        private static bool ElementColorInfosChanged = false;
        private static bool TypeColorOffsetsChanged = false;
        private static bool SkipTiles = false;

        private static void UpdateBuildingsColors()
        {
            foreach (var building in Components.BuildingCompletes)
            {
                OnBuildingsCompletesAdd(building);
            }
        }

        private static void OnElementColorsInfosUpdated(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("Element color infos updated");
            //UpdateBuildingsColors();

            ElementColorInfosChanged = true;
        }

        private static void OnTypeColorOffsetsUpdated(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("Type colors updated");
            //UpdateBuildingsColors();

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

                // ownable
                var ownable = building.GetComponent<Ownable>();

                if (ownable != null)
                {
                    // temporarily disabled for build
                    ownable.ownedTint = color;
                    ownable.unownedTint = dimmedColor;
                }

                // rationbox
                var rationBox = building.GetComponent<RationBox>();

                if (rationBox != null)
                {
                    rationBox.filterTint = color;
                    rationBox.noFilterTint = dimmedColor; 
                }
            }
            else if (!SkipTiles || SkipTiles && buildingName != "Tile" && buildingName !=  "MeshTile" && buildingName != "InsulationTile" && buildingName != "GasPermeableMembrane")
            {
                Debug.LogError($"Can't find KAnimControllerBase component in {buildingName}");
            }
        }

        // not subscribed or used yet
        private static void OnBuildingsCompletesRemove(BuildingComplete building)
        {
            //var storageLocker = building.GetComponent<StorageLocker>();

            //storageLocker.
        }
    }
}
