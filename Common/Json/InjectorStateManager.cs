using Common.Data;

namespace Common.Json
{
    public class InjectorStateManager
    {
        public InjectorStateManager(JsonManager manager)
        {
            _manager = manager;
        }

        private JsonManager _manager;

        public InjectorState LoadState()
        {
            return _manager.Deserialize<InjectorState>(Paths.InjectorStatePath);
        }

        public void SaveState(InjectorState state)
        {
            _manager.Serialize(state, Paths.InjectorStatePath);
        }
    }
}
