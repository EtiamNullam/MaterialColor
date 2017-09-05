using MaterialColor.Common.Data;
using System.Collections.Generic;

namespace MaterialColor.Common.Json
{
    public class ElementColorInfosManager
    {
        public ElementColorInfosManager(JsonManager manager)
        {
            _manager = manager;
        }

        private JsonManager _manager;

        public void SaveElementsColorInfo(Dictionary<SimHashes, ElementColorInfo> dictionary, string path = null)
        {
            if (path == null)
            {
                path = Paths.ElementColorInfosPath;
            }

            _manager.Serialize(dictionary, path);
        }

        public Dictionary<SimHashes, ElementColorInfo> LoadElementColorInfos(string path = null)
        {
            if (path == null)
            {
                path = Paths.ElementColorInfosPath;
            }

            return _manager.Deserialize<Dictionary<SimHashes, ElementColorInfo>>(path);
        }
    }
}
