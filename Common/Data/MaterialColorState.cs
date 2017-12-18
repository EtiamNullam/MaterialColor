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

        // not really used, remove?
        public bool ShowDetailedErrorInfo { get; set; }

        public bool RemoteDoorsEnabled { get; set; } = false;
    }
}
