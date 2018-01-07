using Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Common.Data
{
    public class DraggableUIState
    {
        private Dictionary<string, SerializeableVector2> WindowPositions
            => _windowPositions ?? (_windowPositions = LoadFile());

        private Dictionary<string, SerializeableVector2> _windowPositions;

        public void SaveWindowPosition(GameObject window, Vector2 position)
        {
            var key = ExtractKey(window);

            WindowPositions[key] = VectorToTuple(position);

            UpdateFile();
        }

        public bool LoadWindowPosition(GameObject window, out Vector2 position)
        {
            var key = ExtractKey(window);

            SerializeableVector2 sVector2;

            var result = WindowPositions.TryGetValue(key, out sVector2);

            position = sVector2.ToVector2();

            return result;
        }

        private string ExtractKey(GameObject window)
            => window.name;

        private void UpdateFile()
        {
            try
            {
                _jsonManager.Serialize(WindowPositions, Paths.DraggableUIStatePath);
            }
            catch (Exception e)
            {

                State.Logger.Log("Draggable UI state save failed.");
                State.Logger.Log(e);
            }
        }

        private Dictionary<string, SerializeableVector2> LoadFile()
        {
            try
            {
                return _jsonManager.Deserialize<Dictionary<string, SerializeableVector2>>(Paths.DraggableUIStatePath);
            }
            catch (Exception e)
            {
                State.Logger.Log("Draggable UI state load failed.");
                State.Logger.Log(e);

                return new Dictionary<string, SerializeableVector2>();
            }
        }

        private JsonManager _jsonManager = new JsonManager();

        private SerializeableVector2 VectorToTuple(Vector2 vector)
            => new SerializeableVector2(vector.x, vector.y);
    }
}
