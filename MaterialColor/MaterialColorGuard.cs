using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using UnityEngine;
using Common;

namespace MaterialColor
{
    public static class MaterialColorGuard
    {
        public static Dictionary<KAnimControllerBase, Color32> AssociatedColors = new Dictionary<KAnimControllerBase, Color32>();

        public static Dictionary<string, Color32> TypesStandardColor = new Dictionary<string, Color32>
        {
            // new, tested
            { "Ladder",                     new Color32(110, 50, 0, 0xFF) },
            { "Outhouse",                   new Color32(70, 40, 0, 0xFF) },
            { "FlowerVase",                 new Color32(0, 0, 0, 0xFF) },
            { "ManualPressureDoor",         new Color32(50, 50, 0, 0xFF) },
            { "StorageLocker",              new Color32(40, 60, 0, 0xFF) },
            // verify with color
            { "Bed",                        new Color32(40, 30, 0, 0xFF) }, // hint: ownable
            { "Headquarters",               new Color32(30, 50, 0, 0xFF) }, // spawn portal
            { "RationBox",                  new Color32(110, 110, 0, 0xFF) },
            { "Grave",                new Color32(0, 0, 0, 0xFF) },
            { "GasValve",                new Color32(0, 70, 0, 0xFF) },

            // proper name, probably wrong color
            { "TemperatureControlledSwitch",                new Color32(40, 0, 0, 0xFF) },
            { "PressureSwitchGas",                new Color32(40, 0, 0, 0xFF) },
            { "MineralDeoxidizer",                new Color32(0, 70, 0, 0xFF) },
            { "Shower",              new Color32(0, 0, 0, 0xFF) },
            { "InsulatedGasConduit",              new Color32(0, 0, 0, 0xFF) }, // probably changes on overlay change

            // old
            { "Battery",            new Color32(90, 100, 0, 0xFF) },
            { "Door",               new Color32(50, 50, 0, 0xFF) }, // obsolete
            { "Conditioner",        new Color32(0, 0, 0, 0xFF) },
            { "Pump",               new Color32(0, 40, 15, 0xFF) },
            { "Refrigerator",       new Color32(40, 80, 0, 0xFF) },
            { "MedicalCot",         new Color32(0, 0, 0, 0xFF) },
            { "Lavatory",           new Color32(0, 0, 0, 0xFF) },

            // in progress (old)
            { "Tile",              new Color32(0xFF, 0xFF, 0, 0xFF) },

            // future
        };

        // set at injector
        public static bool ShowMissingTypes = false;
        public static bool ShowMissingElements = false;

        // is saved to json
        public static Dictionary<SimHashes, ElementColorInfo> ElementColorInfos = new Dictionary<SimHashes, ElementColorInfo>();

        [Obsolete("Use ElementsColorInfo")]
        public static Color32 GetMaterialColor(KAnimControllerBase kAnimController)
        {
            var elementID = kAnimController.GetComponent<PrimaryElement>().ElementID;

            Color32 result = 0xC12B93.ToColor32();  // magenta

            switch (elementID)
            {
                case SimHashes.Granite:
                    result = 0x7D7E7D.ToColor32();  // gray
                    break;
                case SimHashes.SandStone:
                    result = new Color32(254, 228, 84, 0xFF);
                    //result = 0x513D32.ToColor32();  // deep brown
                    break;
                case SimHashes.Obsidian:
                    // needs to be darker
                    result = 0x113916.ToColor32();  // dark green
                    break;
                case SimHashes.IgneousRock:
                    result = 0x404142.ToColor32();  // dark gray
                    break;
                case SimHashes.SedimentaryRock:
                    result = 0xBBBABA.ToColor32();  // light gray
                    break;
                case SimHashes.Katairite:
                    // abyssalite
                    result = 0x3F7699.ToColor32();  // lake blue
                    break;
                case SimHashes.GoldAmalgam:
                    //result = 0xEBC621.ToColor32();  // yellow
                    result = new Color32(180, 180, 0, 0xFF);
                    break;
                case SimHashes.Copper:
                    result = 0xB26B44.ToColor32();  // copper colour
                    break;
                case SimHashes.IronOre:
                    result = new Color32(201, 70, 14, 0xFF);
                    break;
                case SimHashes.Wolframite:
                    result = 0x1A5AB6.ToColor32();  // blue
                    break;
            }

            // resources test
            //using (var writer = new System.Resources.ResourceWriter("resourcetest"))
            //{
            //    writer.AddResource(SimHashes.Granite.ToString(), 0x7D7E7D.ToColor32());

            //    writer.Close();
            //}

            //using (var reader = new ResourceReader("resourcetest"))
            //{
            //    var enumerator = reader.GetEnumerator();

            //    enumerator.
            //}
            //

            // test add (ladder only) - correct
            //result = result.Add(new Color32(73, 31, 20, 0xFF));
            //

            // test divide (ladder only)
            //result = result.Divide(new Color32(73, 31, 20, 0xFF));
            //

            return result;
        }

        [Obsolete("Use ElementsColorInfo")]
        public static Color32Multiplier GetMaterialColorMultiplier(KAnimControllerBase kAnimController)
        {
            var elementID = kAnimController.GetComponent<PrimaryElement>().ElementID;

            switch (elementID)
            {
                // raw minerals
                case SimHashes.Katairite: return new Color32Multiplier(0.1f, 0.7f, 0.5f); // abyssalite
                case SimHashes.IgneousRock: return new Color32Multiplier(0.25f, 0.25f, 0.25f);
                case SimHashes.SedimentaryRock: return new Color32Multiplier(0.7f, 0.7f, 0.7f);
                case SimHashes.Granite: return new Color32Multiplier(1, 1, 1);
                case SimHashes.SandStone: return new Color32Multiplier(1.1f, 0.6f, 0.3f);
                case SimHashes.Obsidian: return new Color32Multiplier(0.3f, 0.4f, 0.3f);
                // raw metals
                case SimHashes.Cuprite: return new Color32Multiplier(1, 0.5f, 0.3f); // copper ore
                case SimHashes.IronOre: return new Color32Multiplier(0.6f, 0.4f, 0.3f);
                case SimHashes.GoldAmalgam: return new Color32Multiplier(0.7f, 0.95f, 0.4f);
                case SimHashes.Wolframite: return new Color32Multiplier(0.3f, 0.2f, 0.9f);
                // alloys?
                case SimHashes.Steel: return new Color32Multiplier(1, 1, 1);
                case SimHashes.Iron: return new Color32Multiplier(0.7f, 0.7f, 0.7f);
                case SimHashes.Copper:
                case SimHashes.Tungsten:
                case SimHashes.Gold:
                case SimHashes.Electrum:
                case SimHashes.FoolsGold:
                case SimHashes.Diamond:
                // other
                case SimHashes.CarbonFibre:
                case SimHashes.Brick:
                case SimHashes.SteelDoor:
                default:
                    throw new Exception(elementID.ToString());
                    //return new Color32Multiplier(1, 0.1f, 0.8f);
            }

        }

        [Obsolete]
        public static string Dim = "Dark";

        [Obsolete("Use GetMaterialColor()")]
        public static void UpdateMaterialColor(KAnimControllerBase kAnimController, string objectTypeName)
        {
            var result = GetMaterialColor(kAnimController, objectTypeName);

            kAnimController.TintColour = result;
        }

        public static Color32 GetMaterialColor(KAnimControllerBase kAnimController, string objectTypeName)
        {
            var primaryElement = kAnimController.GetComponent<PrimaryElement>().ElementID;
            if (!ElementColorInfos.TryGetValue(primaryElement, out ElementColorInfo elementColorInfo))
            {
                if (ShowMissingElements)
                {
                    Debug.LogError($"Can't find {primaryElement} color info");
                }
                return new Color32(0xFF, 0, 0xFF, 0xFF);
            }

            if (!TypesStandardColor.TryGetValue(objectTypeName, out Color32 typeStandardColor))
            {
                if (ShowMissingTypes)
                {
                    Debug.LogError($"Can't find {objectTypeName} type color");
                }
                return new Color32(0xFF, 0, 0xFF, 0xFF);
            }

            var whiteColorForObject = typeStandardColor.TintToWhite();

            // use to set object color offset (dark wont dim here)
            kAnimController.TintColour = whiteColorForObject;
            return whiteColorForObject;
            //

            var materialColor = whiteColorForObject.Multiply(elementColorInfo.ColorMultiplier).SetBrightness(elementColorInfo.Brightness);

            // test - everything should be gray
            //kAnimController.TintColour = TintToWhite(TypeStandardColor[objectTypeName]);

            return materialColor;
        }

        [Obsolete]
        public static void StartColorGuard(KAnimControllerBase kAnimController)
        {
            var targetColor = GetMaterialColor(kAnimController);

            AssociatedColors.Add(kAnimController, targetColor);

            MaterialColorGuard.CheckColor(kAnimController.TintColour, kAnimController);

            ////target.OnTintChanged += CheckColor;

            if (!_isSubscribedToOverlayChanged)
            {
                OverlayScreen.OnOverlayChanged += OnOverlayChange;
                _isSubscribedToOverlayChanged = true;
            }
        }

        // looks like its not really needed
        private static bool _isSubscribedToOverlayChanged = false;

        // looks like its not really needed
        private static void OnOverlayChange(SimViewMode newMode)
        {
            // verify
            if (newMode == SimViewMode.Priorities)
            {
                //throw new Exception();
            }
        }

        // low data on event, maybe increase it? (pass KAnimControllerBase)
        [Obsolete]
        private static void CheckColor(Color32 newColor, KAnimControllerBase kAnimController)
        {
            Color32 color;

            if (newColor == Color.white && AssociatedColors.TryGetValue(kAnimController, out color))
            {
                kAnimController.TintColour = color;
            }
        }

        [Obsolete]
        public static void StopColorGuard(KAnimControllerBase target)
        {
            var animController = target.GetComponent<KAnimControllerBase>();

            AssociatedColors.Remove(animController);

            ////animController.OnTintChanged -= CheckColor;
        }

        // do it once
        public static void ExportDictionaryToJson()
        {
            var serializer = Newtonsoft.Json.JsonSerializer.Create(new Newtonsoft.Json.JsonSerializerSettings { Formatting = Newtonsoft.Json.Formatting.Indented });

            using (var textWriter = new System.IO.StreamWriter("ElementsColorInfo.json"))
            {
                using (var jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter))
                {
                    serializer.Serialize(jsonWriter, ElementColorInfos);
                    jsonWriter.Close();
                }
                textWriter.Close();
            }


            // test - correct (local path)
            //var info = new System.IO.FileInfo("test");
        }

        // using WCF (servicemodel) breaks everything
        // convert to instance and clear on finalize/dispose
        //public static void StartListenForColorReload()
        //{
        //    _serviceHost = new WCF.ColorReloaderHost();

        //    WCF.ColorReloaderService.OnColorsReload += OnServiceColorsReload;
        //}

        //private static WCF.ColorReloaderHost _serviceHost;

        public static void OnServiceColorsReload(object sender, EventArgs e)
        {
            Debug.LogError("OnServiceColorsReload");
        }
    }
}