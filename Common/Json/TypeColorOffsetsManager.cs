using System.Collections.Generic;
using UnityEngine;

namespace Common.Json
{
    public class TypeColorOffsetsManager
    {
        public TypeColorOffsetsManager(JsonManager manager)
        {
            _manager = manager;
        }

        private JsonManager _manager;

        public void SaveTypesColors(Dictionary<string, Color32> dictionary, string path = null)
        {
            if (path == null)
            {
                path = Paths.TypeColorsPath;
            }

            _manager.Serialize(dictionary, path);
        }

        public Dictionary<string, Color32> LoadTypeColorOffsets(string path = null)
        {
            if (path == null)
            {
                path = Paths.TypeColorsPath;
            }

            return _manager.Deserialize<Dictionary<string, Color32>>(path);
        }
    }
}
