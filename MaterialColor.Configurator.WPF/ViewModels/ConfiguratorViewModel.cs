using MaterialColor.Common;
using MaterialColor.Common.Data;
using MaterialColor.Common.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace MaterialColor.Configurator.WPF.ViewModels
{
    public class ConfiguratorViewModel : BindableBase
    {
        public ConfiguratorViewModel(ConfiguratorStateManager stateManager)
        {
            _stateManager = stateManager;

            _logger = new Common.IO.Logger(Paths.ConfiguratorLogFileName);

            TryLoadLastAppState();

            ApplyCommand = new DelegateCommand(Apply);
            ExitCommand = new DelegateCommand(App.Current.Shutdown);
        }

        public MaterialColorState MaterialState
        {
            get => _materialState;
            set => SetProperty(ref _materialState, value);
        }

        private MaterialColorState _materialState;

        public OnionState OnionState
        {
            get => _onionState;
            set => SetProperty(ref _onionState, value);
        }

        private OnionState _onionState;

        private Common.IO.Logger _logger;
        private ConfiguratorStateManager _stateManager;

        public DelegateCommand ApplyCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, $"{_status}\n[{DateTime.Now.TimeOfDay}]: {value}".Trim());
        }

        private string _status;

        private bool TryLoadLastAppState()
        {
            try
            {
                MaterialState = _stateManager.LoadMaterialColorState();
                OnionState = _stateManager.LoadOnionState();

                return true;
            }
            catch (Exception e)
            {
                var message = "Can't load last state";
                _logger.Log($"{message}\n{e.Message}\n{e.StackTrace}");

                MaterialState = new MaterialColorState();
                OnionState = new OnionState();

                return false;
            }
        }

        private void Apply()
        {
            try
            {
                Common.IO.IOHelper.EnsureDirectoryExists(Common.Paths.ConfigDirectory);
            }
            catch (Exception e)
            {
                var message = "Can't create or access directory for state to save.";

                _logger.Log($"{message}\n{e.Message}\n{e.StackTrace}");
                Status = message;

                return;
            }

            try
            {
                _stateManager.SaveMaterialColorState(MaterialState);
                _stateManager.SaveOnionState(OnionState);
            }
            catch (Exception e)
            {
                var message = $"Can't save current state.";

                _logger.Log($"{message}\n{e.Message}\n{e.StackTrace}");
                Status = message;

                return;
            }

            Status = "State applied.";
        }
    }
}
