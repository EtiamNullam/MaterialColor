using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
    public class MaterialColorState
    {
        public bool Enabled { get; set; } = true;

        public ColorMode ColorMode { get; set; } = ColorMode.Json;

        public bool ShowMissingElementColorInfos { get; set; }
        public bool ShowMissingTypeColorOffsets { get; set; }
        public bool ShowBuildingsAsWhite { get; set; }

        public bool LegacyTileColorHandling { get; set; } = false;

        // gas overlay
        public float MinimumGasColorIntensity { get; set; } = 0.25f;
        public float GasPressureStart { get; set; } = 0.1f;

        public float GasPressureEnd
        {
            get
            {
                return _gasPressureEnd;
            }
            set
            {
                if (_gasPressureEnd <= 0)
                {
                    _gasPressureEnd = float.Epsilon;
                }
                else
                {
                    _gasPressureEnd = value;
                }
            }
        }

        private float _gasPressureEnd = 2.5f;
        //

        // not really used, remove?
        public bool ShowDetailedErrorInfo { get; set; }
    }
}
