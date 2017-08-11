using Common;
using Common.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MaterialColor
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

        // wip
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


        // skips alpha
        public static Color32 Substract(this Color32 a, Color32 b)
        {
            a.r = (byte)Math.Max(a.r - b.r, 0);
            a.g = (byte)Math.Max(a.g - b.g, 0);
            a.b = (byte)Math.Max(a.b - b.b, 0);

            return a;
        }

        // skips alpha
        public static Color32 Add(this Color32 a, Color32 b)
        {
            a.r = (byte)Math.Min(a.r + b.r, 0xFF);
            a.g = (byte)Math.Min(a.g + b.g, 0xFF);
            a.b = (byte)Math.Min(a.b + b.b, 0xFF);

            return a;
        }

        // skips alpha
        public static Color32 Multiply(this Color32 a, Color32 b)
        {
            a.r *= b.r;
            a.g *= b.g;
            a.b *= b.b;

            return a;
        }

        // skips alpha
        public static Color32 Divide(this Color32 a, Color32 b)
        {
            if (b.r == 0) b.r++;
            if (b.g == 0) b.g++;
            if (b.b == 0) b.b++;

            a.r /= b.r;
            a.g /= b.g;
            a.b /= b.b;

            return a;
        }

        public static Color32 Divide(this Color32 color, byte value)
        {
            if (value == 0) throw new Exception("Division by zero");

            color.r /= value;
            color.g /= value;
            color.b /= value;

            return color;
        }
    }
}
