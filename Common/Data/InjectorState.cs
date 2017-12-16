using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
    public class InjectorState
    {
        public bool InjectMaterialColor { get; set; } = true;
        public bool InjectMaterialColorOverlayButton { get; set; } = true;
        public bool InjectOnion { get; set; } = true;

        public bool CustomSensorRanges { get; set; } = true;
        public float MaxSensorTemperature { get; set; } = 1273.15f;
        public float MaxGasSensorPressure { get; set; } = 25;
        public float MaxLiquidSensorPressure { get; set; } = 10000;

        public bool InjectRemoteDoors { get; set; } = false;
        public bool EnableDebugConsole { get; set; } = false;
    }
}
