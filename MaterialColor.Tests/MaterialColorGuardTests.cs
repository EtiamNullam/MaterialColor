using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace MaterialColor.Tests
{
    // TODO: move appropriate methods to color32extensionstests
    [TestFixture]
    public class MaterialColorGuardTests
    {
        //[Test]
        public void Substract_Basic()
        {
            var a = new Color32(255, 128, 0, 255);
            var b = new Color32(200, 100, 20, 255);

            var result = a.Substract(b);

            Assert.AreEqual(new Color32(0, 0, 0, 0), result);
        }

        //[Test]
        public void Add_Basic()
        {
            var a = new Color32(255, 128, 0, 255);
            var b = new Color32(200, 100, 20, 255);

            var result = a.Add(b);

            Assert.AreEqual(new Color32(0, 0, 0, 0), result);
        }
    }
}
