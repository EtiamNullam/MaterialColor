using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaterialColor.Tests
{
    [TestFixture]
    public class Color32ExtensionsTests
    {
        [Test]
        public void ToColor32_Basic()
        {
            var magentaInHex = 0xC12B93;

            var magentaInColor32 = magentaInHex.ToColor32();

            Assert.AreEqual(magentaInColor32, new Color32(0xC1, 0x2B, 0x93, 0xFF));
        }

        [TestCase(0xABCDEF)]
        [TestCase(0xFEDCBA)]
        [TestCase(0)]
        [TestCase(0xFFFFFF)]
        public void ToHex_Basic(int hexValue)
        {
            Assert.AreEqual(hexValue.ToColor32().ToHex(), hexValue);
        }

        [TestCase(0xFFFFFF, ExpectedResult = 1f)]
        [TestCase(0x0, ExpectedResult = 0f)]
        [TestCase(0x333333, ExpectedResult = 0.2f)]
        [TestCase(0xFF1530, ExpectedResult = 1f)]
        public float GetBrightness_Basic(int hexColorValue)
        {
            var color = hexColorValue.ToColor32();

            var result = color.GetBrightness();

            return result;
        }

        [TestCase(0x333333, 0.4f, ExpectedResult = 0x666666)]
        public int SetBrightness_Basic(int inputHexColorValue, float targetBrightness)
        {
            var color = inputHexColorValue.ToColor32();

            var newColor = color.SetBrightness(targetBrightness);
            var hexResult = newColor.ToHex();

            return hexResult;
        }
    }
}
