using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using UnityEngine;
using Common;
using Common.DataModels;

namespace MaterialColor
{
    public static class MaterialColorGuard
    {
        public static Dictionary<KAnimControllerBase, Color32> AssociatedColors = new Dictionary<KAnimControllerBase, Color32>();

        public static Dictionary<string, Color32> TypeColorOffsets = new Dictionary<string, Color32>();

        //public static bool ShowMissingTypes = false;
        //public static bool ShowMissingElements = false;
        //public static bool SetColorableObjectsAsWhite = false;

        public static ConfiguratorState ConfiguratorState = new ConfiguratorState();

        public static Dictionary<SimHashes, ElementColorInfo> ElementColorInfos = new Dictionary<SimHashes, ElementColorInfo>();

        public static Color32 GetMaterialColor(KAnimControllerBase kAnimController, string objectTypeName)
        {
            var primaryElementHash = kAnimController.GetComponent<PrimaryElement>().ElementID;

            var elementColorInfo = GetMaterialColorInfo(primaryElementHash);
            var typeStandardColor = GetTypeStandardColor(objectTypeName);

            var colorOffsetForWhite = typeStandardColor.TintToWhite();

            if (ConfiguratorState.ShowBuildingsAsWhite)
            //if (SetColorableObjectsAsWhite)
            {
                kAnimController.TintColour = colorOffsetForWhite;
                return colorOffsetForWhite;
            }

            var materialColor = colorOffsetForWhite.Multiply(elementColorInfo.ColorMultiplier).SetBrightness(elementColorInfo.Brightness);

            return materialColor;
        }

        // TODO: refactor
        public static Color32 GetTypeStandardColor(string typeName)
        {
            if (!TypeColorOffsets.TryGetValue(typeName, out Color32 typeStandardColor))
            {
                //if (ShowMissingTypes)
                if (ConfiguratorState.ShowMissingTypeColorOffsets)
                {
                    Debug.LogError($"Can't find {typeName} type color");
                    return new Color32(0xFF, 0, 0xFF, 0xFF);
                }
                return new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            }
            return typeStandardColor;
        }

        // TODO: refactor
        public static ElementColorInfo GetMaterialColorInfo(SimHashes materialHash)
        {
            if (!ElementColorInfos.TryGetValue(materialHash, out ElementColorInfo elementColorInfo))
            {
                if (ConfiguratorState.ShowMissingElementColorInfos)
                //if (ShowMissingElements)
                {
                    Debug.LogError($"Can't find {materialHash} color info");
                    return new ElementColorInfo(new Color32Multiplier(1, 0, 1), 1);
                }
                return new ElementColorInfo(new Color32Multiplier(1, 1, 1), 1);
            }

            return elementColorInfo;
        }

        // TODO: refactor
        public static Color GetCellMaterialColor(SimHashes materialHash)
        {
            var colorInfo = GetMaterialColorInfo(materialHash);

            // change to extension method?
            var result = new Color(
                colorInfo.ColorMultiplier.Red,
                colorInfo.ColorMultiplier.Green,
                colorInfo.ColorMultiplier.Blue
                ) * colorInfo.Brightness;

            result.a = byte.MaxValue;

            return result;
        }
    }
}