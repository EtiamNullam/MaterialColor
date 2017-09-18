using Common.Data;

namespace Common.Json
{
    public class ConfiguratorStateManager
    {
        public ConfiguratorStateManager(JsonManager manager)
        {
            _manager = manager;
        }

        private JsonManager _manager;

        public MaterialColorState LoadMaterialColorState()
        {
            return _manager.Deserialize<MaterialColorState>(Paths.MaterialColorStatePath);
        }

        public void SaveMaterialColorState(MaterialColorState state)
        {
            _manager.Serialize(state, Paths.MaterialColorStatePath);
        }

        public OnionState LoadOnionState()
        {
            return _manager.Deserialize<OnionState>(Paths.OnionStatePath);
        }

        public void SaveOnionState(OnionState state)
        {
            _manager.Serialize(state, Paths.OnionStatePath);
        }
    }
}