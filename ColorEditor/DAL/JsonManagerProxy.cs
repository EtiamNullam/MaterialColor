using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Json;
using MaterialColor;
using UnityEngine;
using Common;

namespace ColorEditor.WPF.DAL
{
    public class JsonManagerProxy
    {
        private JsonManager _manager = new JsonManager();

        public Dictionary<SimHashes, ElementColorInfo> LoadElementColorInfos()
        {
            return _manager.Deserialize<Dictionary<SimHashes, ElementColorInfo>>(Common.DefaultPaths.ElementColorInfosPath);
        }

        public Dictionary<string, Color32> LoadTypeColors()
        {
            return _manager.Deserialize<Dictionary<string, Color32>>(Common.DefaultPaths.TypeColorsPath);
        }
    }
}
