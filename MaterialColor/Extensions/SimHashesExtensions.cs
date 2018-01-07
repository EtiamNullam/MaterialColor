using Common.Data;
using MaterialColor.Helpers;
using UnityEngine;

namespace MaterialColor.Extensions
{
    public static class SimHashesExtensions
    {
        public static Color32 GetMaterialColorForType(this SimHashes material, string objectTypeName)
        {
            Color32 typeStandardColor;
            if (!ColorHelper.TryGetTypeStandardColor(objectTypeName, out typeStandardColor))
            {
                if (State.ConfiguratorState.ShowMissingTypeColorOffsets)
                {
                    Debug.LogError($"Can't find <{objectTypeName}> type color");
                    return typeStandardColor;
                }
            }

            var colorOffsetForWhite = typeStandardColor.TintToWhite();

            if (State.ConfiguratorState.ShowBuildingsAsWhite)
            {
                return colorOffsetForWhite;
            }

            var elementColorInfo = material.GetMaterialColorInfo();
            var materialColor = colorOffsetForWhite.Multiply(elementColorInfo.ColorMultiplier).SetBrightness(elementColorInfo.Brightness);

            return materialColor;
        }

        public static ElementColorInfo GetMaterialColorInfo(this SimHashes materialHash)
        {
            ElementColorInfo elementColorInfo;
            if (State.ElementColorInfos.TryGetValue(materialHash, out elementColorInfo))
                return elementColorInfo;

            if (!State.ConfiguratorState.ShowMissingElementColorInfos)
                return new ElementColorInfo(Color32Multiplier.One);


            Debug.LogError($"Can't find <{materialHash}> color info");
            return new ElementColorInfo(new Color32Multiplier(1, 0, 1), 1);
        }

        public static Color ToCellMaterialColor(this SimHashes material)
        {
            var colorInfo = material.GetMaterialColorInfo();

            var result = new Color(
                colorInfo.ColorMultiplier.Red,
                colorInfo.ColorMultiplier.Green,
                colorInfo.ColorMultiplier.Blue
                ) * colorInfo.Brightness;

            result.a = byte.MaxValue;

            return result;
        }

        public static Color32 ToDebugColor(this SimHashes material)
        {
            var element = ElementLoader.FindElementByHash(material);

            var debugColor = element.substance.debugColour;

            debugColor.a = byte.MaxValue;

            return debugColor;
        }
    }
}
