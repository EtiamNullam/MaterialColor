using Common.Data;
using Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaterialColor.IO
{
    public class JsonFileLoader
    {
        public JsonFileLoader(JsonManager jsonManager)
        {
            InitializeManagers(jsonManager);
        }

        private ConfiguratorStateManager _configuratorStateManager;
        private ElementColorInfosManager _elementColorInfosManager;
        private TypeColorOffsetsManager _typeColorOffsetsManager;

        private void InitializeManagers(JsonManager manager)
        {
            _configuratorStateManager = new ConfiguratorStateManager(manager);
            _elementColorInfosManager = new ElementColorInfosManager(manager);
            _typeColorOffsetsManager = new TypeColorOffsetsManager(manager);
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
                var message = "Can't load configurator state.\n" + ex.Message + '\n';

                if (State.ConfiguratorState != null && State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + ex.StackTrace;
                }
                Debug.LogError(message);

                state = new MaterialColorState();

                return false;
            }
        }

        public bool TryLoadElementColorInfos(out Dictionary<SimHashes, ElementColorInfo> elementColorInfos)
        {
            try
            {
                elementColorInfos = _elementColorInfosManager.LoadElementColorInfos();
                return true;
            }
            catch (Exception e)
            {
                var message = "Can't load ElementColorInfos\n" + e.Message + '\n';

                if (State.ConfiguratorState != null && State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + e.StackTrace;
                }

                Debug.LogError(message);

                elementColorInfos = new Dictionary<SimHashes, ElementColorInfo>();
                return false;
            }
        }

        public bool TryLoadTypeColorOffsets(out Dictionary<string, Color32> typeColorOffsets)
        {
            try
            {
                typeColorOffsets = _typeColorOffsetsManager.LoadTypeColorOffsets();
                return true;
            }
            catch (Exception e)
            {
                var message = "Can't load TypeColorOffsets\n" + e.Message + '\n';

                if (State.ConfiguratorState != null && State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + e.StackTrace;
                }

                Debug.LogError(message);

                typeColorOffsets = new Dictionary<string, Color32>();
                return false;
            }
        }
    }
}
