using Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injector.WPF.DAL
{
    public class JsonManagerProxy
    {
        public const string LastStatePath = "LastInjectorState.json";

        private JsonManager _manager = new JsonManager();

        public bool LoadLastState()
        {
            return _manager.Deserialize<bool>(LastStatePath);
        }

        public void SaveState(bool state)
        {
            _manager.Serialize(state, LastStatePath);
        }
    }
}
