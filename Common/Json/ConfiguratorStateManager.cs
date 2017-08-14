using Common.Data;

namespace Common.Json
{
    public class ConfiguratorStateManager
    {
        private JsonManager _manager = new JsonManager();

        public ConfiguratorState LoadState()
        {
            return _manager.Deserialize<ConfiguratorState>(DefaultPaths.ConfiguratorStatePath);
        }

        public void SaveState(ConfiguratorState state)
        {
            _manager.Serialize(state, DefaultPaths.ConfiguratorStatePath);
        }
    }
}