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

        public bool OnionEnabled
        {
            get
            {
                return _onionState.Enabled;
            }
            set
            {
                _onionState.Enabled = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(OnionCustomWorldSizeEnabled));
                RaisePropertyChanged(nameof(OnionCustomSeedsEnabled));
                RaisePropertyChanged(nameof(OnionCustomMaxCameraEnabled));
            }
        }

        public bool OnionCustomMaxCameraEnabled
        {
            get => _onionState.Enabled && OnionCustomMaxCamera;
        }

        public bool OnionCustomMaxCamera
        {
            get => _onionState.CustomMaxCameraDistance;
            set
            {
                _onionState.CustomMaxCameraDistance = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(OnionCustomMaxCameraEnabled));
            }
        }

        public bool OnionCustomWorldSizeEnabled
        {
            get
            {
                return _onionState.Enabled && OnionCustomWorldSize;
            }
        }

        public bool OnionCustomWorldSize
        {
            get
            {
                return _onionState.CustomWorldSize;
            }
            set
            {
                _onionState.CustomWorldSize = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(OnionCustomWorldSizeEnabled));
            }
        }

        public bool OnionCustomSeedsEnabled
        {
            get => _onionState.Enabled && OnionCustomSeeds;
        }

        public bool OnionCustomSeeds
        {
            get => _onionState.CustomSeeds;
            set
            {
                _onionState.CustomSeeds = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(OnionCustomSeedsEnabled));
            }
        }

        private Common.IO.Logger _logger;
        private ConfiguratorStateManager _stateManager;

        public DelegateCommand ApplyCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }

        public string Status
        {
            get => _status;
            set
            {
                SetProperty(ref _status, $"{_status}\n[{DateTime.Now.TimeOfDay}]: {value}".Trim());
                _logger.Log(value);
            }
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
                Status = "Can't load last state";

                _logger.Log(e);

                MaterialState = new MaterialColorState();
                OnionState = new OnionState();

                return false;
            }
        }

        private void Apply()
        {
            try
            {
                Common.IO.IOHelper.EnsureDirectoryExists(Common.Paths.MaterialConfigPath);
                Common.IO.IOHelper.EnsureDirectoryExists(Common.Paths.OnionConfigPath);
            }
            catch (Exception e)
            {
                Status = "Can't create or access directory for state to save.";

                _logger.Log(e);

                return;
            }

            try
            {
                _stateManager.SaveMaterialColorState(MaterialState);
                _stateManager.SaveOnionState(OnionState);
            }
            catch (Exception e)
            {
                Status = $"Can't save current state.";

                _logger.Log(e);

                return;
            }

            Status = "State applied.";
        }
    }
}
