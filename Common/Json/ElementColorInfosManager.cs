using Common.Data;
using System.Collections.Generic;

namespace Common.Json
{
    public class ElementColorInfosManager
    {
        // TODO: inject instead of creating?
        private JsonManager _manager = new JsonManager();

        // not used
        public void SaveElementsColorInfo(Dictionary<SimHashes, ElementColorInfo> dictionary)
        {
            _manager.Serialize(dictionary, Paths.ElementColorInfosPath);
        }

        public Dictionary<SimHashes, ElementColorInfo> LoadElementColorInfos()
        {
            return _manager.Deserialize<Dictionary<SimHashes, ElementColorInfo>>(Paths.ElementColorInfosPath);
        }
    }
}
