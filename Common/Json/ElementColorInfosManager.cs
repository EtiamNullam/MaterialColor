using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Json
{
    public class ElementColorInfosManager
    {
        // TODO: inject instead of creating?
        private JsonManager _manager = new JsonManager();

        public void SaveElementsColorInfo(Dictionary<SimHashes, ElementColorInfo> dictionary)
        {
            _manager.Serialize(dictionary, DefaultPaths.ElementColorInfosPath);
        }

        public Dictionary<SimHashes, ElementColorInfo> LoadElementColorInfos()
        {
            return _manager.Deserialize<Dictionary<SimHashes, ElementColorInfo>>(DefaultPaths.ElementColorInfosPath);
        }
    }
}
