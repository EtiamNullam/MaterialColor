﻿using MaterialColor.Common.Data;
using System;
using UnityEngine;

namespace MaterialColor.Core.Extensions
{
    public static class Color32Extensions
    {
        public static Color32 ToColor32(this int HexVal)
        {
            var R = (byte)((HexVal >> 16) & 0xFF);
            var G = (byte)((HexVal >> 8) & 0xFF);
            var B = (byte)((HexVal) & 0xFF);

            return new Color32(R, G, B, 0xFF);
        }

        public static int ToHex(this Color32 color)
        {
            return color.r << 16 | color.g << 8 | color.b;
        }

        public static Color32 SetBrightness(this Color32 color, float targetBrightness)
        {
            var currentBrightness = color.GetBrightness();

            var result = color.Multiply(new Color32Multiplier(targetBrightness / currentBrightness));

            return result;
        }

        public static float GetBrightness(this Color32 color)
        {
            float currentBrightness;

            currentBrightness = Math.Max((float)color.r / byte.MaxValue, (float)color.g / byte.MaxValue);
            currentBrightness = Math.Max(currentBrightness, (float)color.b / byte.MaxValue);

            return currentBrightness;
        }

        public static Color32 TintToWhite(this Color32 currentColor)
        {
            var result = new Color32()
            {
                r = (byte)(byte.MaxValue - currentColor.r),
                g = (byte)(byte.MaxValue - currentColor.g),
                b = (byte)(byte.MaxValue - currentColor.b),
                a = byte.MaxValue
            };

            return result;
        }

        public static Color32 Multiply(this Color32 color, Color32Multiplier multiplier)
        {
            color.r = (byte)Mathf.Clamp(color.r * multiplier.Red, byte.MinValue, byte.MaxValue);
            color.g = (byte)Mathf.Clamp(color.g * multiplier.Green, byte.MinValue, byte.MaxValue);
            color.b = (byte)Mathf.Clamp(color.b * multiplier.Blue, byte.MinValue, byte.MaxValue);

            return color;
        }
    }
}
