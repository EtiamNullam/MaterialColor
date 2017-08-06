using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Common.Json
{
    public class TypeColorOffsetsManager
    {
        // inject instead of creating?
        private JsonManager _manager = new JsonManager();

        public void SaveTypesColors(Dictionary<string, Color32> dictionary)
        {
            _manager.Serialize(dictionary, DefaultPaths.TypeColorsPath);
        }

        public Dictionary<string, Color32> LoadTypeColorOffsets()
        {
            return _manager.Deserialize<Dictionary<string, Color32>>(DefaultPaths.TypeColorsPath);
        }
    }
}
