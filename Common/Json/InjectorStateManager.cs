namespace MaterialColor.Common.Json
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
