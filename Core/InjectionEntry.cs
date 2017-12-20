using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class InjectionEntry
    {
        public static void LogicWireBridgeEnter(BuildingDef logicWireBridgeDef)
        {
            logicWireBridgeDef.ObjectLayer = ObjectLayer.Backwall;
        }
    }
}
