using Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Json
{
    public class InjectorStateManager
    {
        private JsonManager _manager = new JsonManager();

        public List<bool> LoadState()
        {
            return _manager.Deserialize<List<bool>>(DefaultPaths.InjectorStatePath);
        }

        public void SaveState(List<bool> state)
        {
            _manager.Serialize(state, DefaultPaths.InjectorStatePath);
        }
    }
}
