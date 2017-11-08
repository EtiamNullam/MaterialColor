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
        public bool EnableDebugConsole { get; set; } = false;
    }
}
