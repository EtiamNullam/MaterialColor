using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaterialColor
{
    public static class InjectionEntry
    {
        public static void Enter()
        {
            //Debug.LogError("Injected");
            //return;
            try
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
                //MaterialColorGuard.TypesStandardColor = jsonManager.LoadTypesColors();
                // test - result: in same directory as main exe
                //Debug.LogError(new System.IO.FileInfo("test").FullName);
                //return;
                //

                Components.BuildingCompletes.OnAdd += OnBuildingsCompletesAdd;

                foreach (var building in Components.BuildingCompletes)
                {
                    OnBuildingsCompletesAdd(building);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Injection failed\n" 
                    + e.Message + '\n'
                    + e.StackTrace
                    );
            }
        }

        // doesnt work on world reload
        public static void EnterEveryUpdate()
        {
            if (ElementColorInfosChanged || TypeColorOffsetsChanged)
            {
                //TryLoadElementColorInfos();
                //TryLoadTypeColorOffsets();

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

        //private static void OnElementColorsInfosUpdated(object sender, FileSystemEventArgs e)
        //{
        //    Debug.LogError("Element color infos updated");
        //    //UpdateBuildingsColors();

        //    ElementColorInfosChanged = true;
        //}

        //private static void OnTypeColorOffsetsUpdated(object sender, FileSystemEventArgs e)
        //{
        //    Debug.LogError("Type colors updated");
        //    //UpdateBuildingsColors();

        //    TypeColorOffsetsChanged = true;
        //}

        //private static FileChangeNotifier _fileChangeNotifier;

        // refrigerator, storagelocker, bed, medicalcot and others needs special case
        // also all pipes
        // (ownable class)
        // tile doesnt have KAnimControllerBase, find another way to tint it
        // TODO: refactor
        private static void OnBuildingsCompletesAdd(BuildingComplete building)
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
            else
            {
                // skip for tile
                if (buildingName != "Tile")
                {
                    Debug.LogError($"Can't find KAnimControllerBase component in {buildingName}");
                }
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
