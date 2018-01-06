using System.Collections.Generic;

namespace Common.Data
{
    public class TemperatureOverlayState
    {
        public bool CustomRangesEnabled { get; set; } = true;

        public List<float> Temperatures => new List<float>
        {
            Aqua, Turquoise, Blue, Green, Lime, Orange, RedOrange, Red
        };

        public float Red { get; set; } = 1800;
        public float RedOrange { get; set; } = 773;
        public float Orange { get; set; } = 373;
        public float Lime { get; set; } = 303;
        public float Green { get; set; } = 293;
        public float Blue { get; set; } = 0.1f;
        public float Turquoise { get; set; } = 273;
        public float Aqua { get; set; } = 0;

        public bool LogThresholds { get; set; } = false;
    }
}
