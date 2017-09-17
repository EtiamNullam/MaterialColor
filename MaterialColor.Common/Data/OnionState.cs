using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialColor.Common.Data
{
    public class OnionState
    {
        public bool Enabled { get; set; } = true;
        public bool CustomWorldSize { get; set; } = false;
        public int Width { get; set; } = 256;
        public int Height { get; set; } = 384;
        public bool Debug { get; set; } = false;
        public bool FreeCamera { get; set; } = true;
        public bool CustomMaxCameraDistance { get; set; } = true;
        public float MaxCameraDistance { get; set; } = 300;
        public bool LogSeed { get; set; } = true;
        public bool CustomSeeds { get; set; } = false;
        public int WorldSeed { get; set; } = 0;
        public int LayoutSeed { get; set; } = 0;
        public int TerrainSeed { get; set; } = 0;
        public int NoiseSeed { get; set; } = 0;
    }
}
