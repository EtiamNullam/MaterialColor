using MaterialColor.Common.Data;

namespace MaterialColor.Common.Json
{
    public class ConfiguratorStateManager
    {
        public ConfiguratorStateManager(JsonManager manager)
        {
            _manager = manager;
        }

        private JsonManager _manager;

        public ConfiguratorState LoadState()
        {
            return _manager.Deserialize<ConfiguratorState>(Paths.ConfiguratorStatePath);
        }

        public void SaveState(ConfiguratorState state)
        {
            _manager.Serialize(state, Paths.ConfiguratorStatePath);
        }
    }
}