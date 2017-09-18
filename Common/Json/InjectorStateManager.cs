using System.Collections.Generic;

namespace Common.Json
{
    public class InjectorStateManager
    {
        public InjectorStateManager(JsonManager manager)
        {
            _manager = manager;
        }

        private JsonManager _manager;

        public List<bool> LoadState()
        {
            return _manager.Deserialize<List<bool>>(Paths.InjectorStatePath);
        }

        public void SaveState(IEnumerable<bool> state)
        {
            _manager.Serialize(state, Paths.InjectorStatePath);
        }
    }
}
