using MaterialColor.Common.Data;
using System.Collections.Generic;

namespace MaterialColor.Common.Json
{
    public class ElementColorInfosManager
    {
        // TODO: inject instead of creating?
        private JsonManager _manager = new JsonManager();

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
