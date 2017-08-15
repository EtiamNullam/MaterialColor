using Common.Data;
using MaterialColor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaterialColor.Extensions
{
    public static class SimHashesExtensions
    {
        public static Color32 GetMaterialColorForType(this SimHashes material, string objectTypeName)
        {
            var typeStandardColor = ColorHelper.GetTypeStandardColor(objectTypeName);
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
            if (!State.ElementColorInfos.TryGetValue(materialHash, out ElementColorInfo elementColorInfo))
            {
                if (State.ConfiguratorState.ShowMissingElementColorInfos)
                {
                    Debug.LogError($"Can't find <{materialHash}> color info");
                    return new ElementColorInfo(new Color32Multiplier(1, 0, 1), 1);
                }
                return new ElementColorInfo(Color32Multiplier.One);
            }

            return elementColorInfo;
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
    }
}
