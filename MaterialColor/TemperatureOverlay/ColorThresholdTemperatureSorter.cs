using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SimDebugView;

namespace MaterialColor.TemperatureOverlay
{
    class ColorThresholdTemperatureSorter : IComparer<ColorThreshold>
    {
        public int Compare(ColorThreshold x, ColorThreshold y)
        {
            return x.value.CompareTo(y.value);
        }
    }
}
