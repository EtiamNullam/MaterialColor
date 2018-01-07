using Common.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Json
{
    public class JsonFileLoader
    {
        public JsonFileLoader(JsonManager jsonManager, Common.IO.Logger logger = null)
        {
            _logger = logger;

            InitializeManagers(jsonManager);
        }

        private readonly Common.IO.Logger _logger;

        private ConfiguratorStateManager _configuratorStateManager;
        private ElementColorInfosManager _elementColorInfosManager;
        private TypeColorOffsetsManager _typeColorOffsetsManager;

        private void InitializeManagers(JsonManager manager)
        {
            _configuratorStateManager = new ConfiguratorStateManager(manager, _logger);
            _elementColorInfosManager = new ElementColorInfosManager(manager, _logger);
            _typeColorOffsetsManager = new TypeColorOffsetsManager(manager, _logger);
        }

        public bool TryLoadConfiguratorState(out MaterialColorState state)
        {
            try
            {
                state = _configuratorStateManager.LoadMaterialColorState();
                return true;
            }
            catch (Exception ex)
            {
                const string Message = "Can't load configurator state.";

                _logger.Log(ex);
                _logger.Log(Message);

                Debug.LogError(Message);

                state = new MaterialColorState();

                return false;
            }
        }

        public bool TryLoadElementColorInfos(out Dictionary<SimHashes, ElementColorInfo> elementColorInfos)
        {
            try
            {
                elementColorInfos = _elementColorInfosManager.LoadElementColorInfosDirectory();
                return true;
            }
            catch (Exception e)
            {
                const string Message = "Can't load ElementColorInfos";

                Debug.LogError(Message + '\n' + e.Message + '\n');

                State.Logger.Log(Message);
                State.Logger.Log(e);

                elementColorInfos = new Dictionary<SimHashes, ElementColorInfo>();
                return false;
            }
        }

        public bool TryLoadTypeColorOffsets(out Dictionary<string, Color32> typeColorOffsets)
        {
            try
            {
                typeColorOffsets = _typeColorOffsetsManager.LoadTypeColorOffsetsDirectory();
                return true;
            }
            catch (Exception e)
            {
                const string Message = "Can't load TypeColorOffsets";

                Debug.LogError(Message + '\n' + e.Message + '\n');

                State.Logger.Log(Message);
                State.Logger.Log(e);

                typeColorOffsets = new Dictionary<string, Color32>();
                return false;
            }
        }

        public bool TryLoadTemperatureState(out TemperatureOverlayState state)
        {
            try
            {
                state = _configuratorStateManager.LoadTemperatureState();
                return true;
            }
            catch (Exception e)
            {
                _logger.Log(e);
                _logger.Log("Can't load overlay temperature state");

                state = new TemperatureOverlayState();

                return false;
            }
        }
    }
}
