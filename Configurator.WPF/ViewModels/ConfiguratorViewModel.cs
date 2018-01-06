using Common;
using Common.Data;
using Common.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows;

namespace Configurator.WPF.ViewModels
{
    public class ConfiguratorViewModel : BindableBase
    {
        public ConfiguratorViewModel(JsonManager jsonManager)
        {
            _stateManager = new ConfiguratorStateManager(jsonManager, _logger);

            _logger = new Common.IO.Logger(Paths.ConfiguratorLogFileName);

            TryLoadLastAppState();

            ApplyCommand = new DelegateCommand(Apply);
            ExitCommand = new DelegateCommand(Application.Current.Shutdown);
        }

        public MaterialColorState MaterialState
        {
            get { return _materialState; }
            set { SetProperty(ref _materialState, value); }
        }

        private MaterialColorState _materialState;

        public OnionState OnionState
        {
            get { return _onionState; }
            set { SetProperty(ref _onionState, value); }
        }

        private OnionState _onionState;

        public TemperatureOverlayState TemperatureOverlayState
        {
            get { return _temperatureOverlayState; }
            set { SetProperty(ref _temperatureOverlayState, value); }
        }

        private TemperatureOverlayState _temperatureOverlayState;

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
            get { return _onionState.Enabled && OnionCustomMaxCamera; }
        }

        public bool OnionCustomMaxCamera
        {
            get { return _onionState.CustomMaxCameraDistance; }
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
            get { return _onionState.Enabled && OnionCustomSeeds; }
        }

        public bool OnionCustomSeeds
        {
            get { return _onionState.CustomSeeds; }
            set
            {
                _onionState.CustomSeeds = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(OnionCustomSeedsEnabled));
            }
        }

        private readonly Common.IO.Logger _logger;
        private readonly ConfiguratorStateManager _stateManager;

        public DelegateCommand ApplyCommand { get; }
        public DelegateCommand ExitCommand { get; }

        public string Status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, $"{_status}\n[{DateTime.Now.TimeOfDay}]: {value}".Trim());
                _logger.Log(value);
            }
        }

        private string _status;

        private bool TryLoadLastAppState()
        {
            var result = true;

            try
            {
                MaterialState = _stateManager.LoadMaterialColorState();
            }
            catch (Exception e)
            {
                Status = "Can't load last material state";
                
                _logger.Log(e);

                MaterialState = new MaterialColorState();

                    result = false;
            }

            try
            {
                OnionState = _stateManager.LoadOnionState();
            }
            catch (Exception e)
            {
                Status = "Can't load last onion state";

                _logger.Log(e);
                
                OnionState = new OnionState();

                result = false;
            }

            try
            {
                TemperatureOverlayState = _stateManager.LoadTemperatureState();
            }
            catch (Exception e)
            {
                Status = "Can't load last temperature overlay state";

                _logger.Log(e);

                TemperatureOverlayState = new TemperatureOverlayState();

                result = false;
            }

            return result;
        }

        private void Apply()
        {
            _logger.LogProperties(MaterialState);
            _logger.LogProperties(OnionState);

            try
            {
                Common.IO.IOHelper.EnsureDirectoryExists(Common.Paths.MaterialConfigPath);
                Common.IO.IOHelper.EnsureDirectoryExists(Common.Paths.OnionConfigPath);
                Common.IO.IOHelper.EnsureDirectoryExists(Common.Paths.OverlayConfigPath);
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
                _stateManager.SaveTemperatureState(TemperatureOverlayState);
            }
            catch (Exception e)
            {
                Status = "Can\'t save current state.";

                _logger.Log(e);

                return;
            }

            Status = "State applied.";
        }
    }
}
