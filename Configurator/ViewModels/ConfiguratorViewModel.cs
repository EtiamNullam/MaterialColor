using Common.DataModels;
using Common.Json;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.ViewModels
{
    public class ConfiguratorViewModel : BindableBase
    {
        public ConfiguratorViewModel(ConfiguratorStateManager stateManager, ILoggerFacade logger)
        {
            _stateManager = stateManager;
            _logger = logger;

            State = TryLoadLastAppState();

            ApplyCommand = new DelegateCommand(Apply);
        }

        public ConfiguratorState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        private ConfiguratorState _state;

        private ILoggerFacade _logger;
        private ConfiguratorStateManager _stateManager;

        public DelegateCommand ApplyCommand { get; private set; }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, $"[{DateTime.Now.TimeOfDay}]:\n{value}");
        }

        private string _status;

        private ConfiguratorState TryLoadLastAppState()
        {
            try
            {
                return _stateManager.LoadState();
            }
            catch (Exception e)
            {
                _logger.Log($"Can't load last state.\n{e.Message}\n{e.StackTrace}", Category.Exception, Priority.Low);

                return new ConfiguratorState();
            }
        }

        private void Apply()
        {
            try
            {
                Common.IOHelper.EnsureDirectoryExists(Common.DefaultPaths.Directory);
            }
            catch (Exception e)
            {
                var message = "Can't create or access directory for state to save.";

                _logger.Log($"{message}\n{e.Message}\n{e.StackTrace}", Category.Exception, Priority.High);
                Status = message;

                return;
            }

            try
            {
                _stateManager.SaveState(State);
            }
            catch (Exception e)
            {
                var message = $"Can't save current state.";

                _logger.Log($"{message}\n{e.Message}\n{e.StackTrace}", Category.Exception, Priority.High);
                Status = message;

                return;
            }

            Status = "State applied";
        }
    }
}
