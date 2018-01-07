using Common.Data;
using Common.IO;

namespace Common.Json
{
    public class ConfiguratorStateManager : BaseManager
    {
        public ConfiguratorStateManager(JsonManager manager, Logger logger = null) : base(manager, logger) { }

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

        public TemperatureOverlayState LoadTemperatureState()
        {
            return _manager.Deserialize<TemperatureOverlayState>(Paths.TemperatureStatePath);
        }

        public void SaveTemperatureState(TemperatureOverlayState state)
        {
            _manager.Serialize(state, Paths.TemperatureStatePath);
        }
    }
}