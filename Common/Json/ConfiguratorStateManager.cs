using MaterialColor.Common.Data;

namespace MaterialColor.Common.Json
{
    public class ConfiguratorStateManager
    {
        private JsonManager _manager = new JsonManager();

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