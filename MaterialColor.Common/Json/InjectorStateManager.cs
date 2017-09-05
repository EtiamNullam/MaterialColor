namespace MaterialColor.Common.Json
{
    public class InjectorStateManager
    {
        public InjectorStateManager(JsonManager manager)
        {
            _manager = manager;
        }

        private JsonManager _manager;

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
