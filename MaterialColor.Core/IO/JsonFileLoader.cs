using MaterialColor.Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialColor.Core.IO
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

        public void ReloadAll()
        {
            TryLoadElementColorInfos();
            TryLoadTypeColorOffsets();
            TryLoadConfiguratorState();
        }

        public bool TryLoadConfiguratorState()
        {
            try
            {
                State.ConfiguratorState = _configuratorStateManager.LoadMaterialColorState();
                return true;
            }
            catch (Exception ex)
            {
                var message = "Can't load configurator state.\n" + ex.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + ex.StackTrace;
                }
                Debug.LogError(message);

                return false;
            }
        }

        public bool TryLoadElementColorInfos()
        {
            try
            {
                State.ElementColorInfos = _elementColorInfosManager.LoadElementColorInfos();
                return true;
            }
            catch (Exception e)
            {
                var message = "Can't load ElementColorInfos\n" + e.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + e.StackTrace;
                }

                Debug.LogError(message);

                return false;
            }
        }

        public bool TryLoadTypeColorOffsets()
        {
            try
            {
                State.TypeColorOffsets = _typeColorOffsetsManager.LoadTypeColorOffsets();
                return true;
            }
            catch (Exception e)
            {
                var message = "Can't load TypeColorOffsets\n" + e.Message + '\n';

                if (State.ConfiguratorState.ShowDetailedErrorInfo)
                {
                    message += '\n' + e.StackTrace;
                }

                Debug.LogError(message);

                return false;
            }
        }
    }
}
