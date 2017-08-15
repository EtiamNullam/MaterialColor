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

        public bool LoadState()
        {
            return _manager.Deserialize<bool>(Paths.InjectorStatePath);
        }

        public void SaveState(bool state)
        {
            _manager.Serialize(state, Paths.InjectorStatePath);
        }
    }
}
