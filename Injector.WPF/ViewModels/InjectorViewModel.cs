using Common.Json;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injector.WPF.ViewModels
{
    public class InjectorViewModel : BindableBase
    {
        public InjectorViewModel(InjectorStateManager stateManager, FileManager fileManager, DefaultInjector injector)
        {
            PatchCommand = new DelegateCommand(Patch);
            RestoreBackupCommand = new DelegateCommand(RestoreBackup, CanRestoreBackup);

            _stateManager = stateManager;
            _fileManager = fileManager;
            _injector = injector;

            ApplyState(TryLoadLastAppState());
        }

        ~InjectorViewModel()
        {
            _stateManager.SaveState(
                new List<bool>
                {
                    EnableDebugConsole,
                    ShowMissingElementColorInfos,
                    ShowMissingTypeColorOffsets,
                    SkipTiles,
                    ShowWhite
                });
        }

        public DelegateCommand PatchCommand { get; private set; }
        public DelegateCommand RestoreBackupCommand { get; private set; }

        public string Status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value);
            }
        }

        public bool EnableDebugConsole
        {
            get
            {
                return _enableDebugConsole;
            }
            set
            {
                SetProperty(ref _enableDebugConsole, value);
            }
        }

        public bool ShowMissingElementColorInfos
        {
            get
            {
                return _showMissingElementColorInfos;
            }
            set
            {
                SetProperty(ref _showMissingElementColorInfos, value);
            }
        }

        public bool ShowMissingTypeColorOffsets
        {
            get
            {
                return _showMissingTypeColorOffsets;
            }
            set
            {
                SetProperty(ref _showMissingTypeColorOffsets, value);
            }
        }

        public bool SkipTiles
        {
            get
            {
                return _skipTiles;
            }
            set
            {
                SetProperty(ref _skipTiles, value);
            }
        }
        
        public bool ShowWhite
        {
            get
            {
                return _showWhite;
            }
            set
            {
                SetProperty(ref _showWhite, value);
            }
        }

        private InjectorStateManager _stateManager;
        private FileManager _fileManager;
        private DefaultInjector _injector;

        private string _status;
        private bool _enableDebugConsole;
        private bool _showMissingElementColorInfos;
        private bool _showMissingTypeColorOffsets;
        private bool _skipTiles;
        private bool _showWhite;

        public bool CanRestoreBackup()
            => _fileManager.BackupForFileExists(DefaultPaths.DefaultTargetAssemblyPath);

        public void Patch()
        {
            StringBuilder resultInfo = new StringBuilder($"Patch result ({DateTime.Now}):\n");

            if (CanRestoreBackup())
            {
                if (TryRestoreBackup())
                {
                    resultInfo.Append("\tBackup restored.\n");
                }
                else
                {
                    resultInfo.Append("\tBackup failed.\n\tPatch cancelled.\n");
                    return;
                }
            }

            try
            {
                _injector.InjectDefaultAndBackup(EnableDebugConsole);
            }
            catch (Exception e)
            {
                resultInfo.Append("\tInjection failed.\n" + e.Message + "\n" + e.StackTrace);
                Status += resultInfo.ToString();
                return;
            }

            RestoreBackupCommand.RaiseCanExecuteChanged();

            resultInfo.Append("\tOriginal backed up.\n\tOriginal patched.\n\tPatch successful.\n");

            Status += resultInfo.ToString();
        }

        public void RestoreBackup()
        {
            if (TryRestoreBackup())
            {
                Status += $"Backup restore successful ({DateTime.Now}).\n";
            }
            else
            {
                Status += $"Backup restore failed ({DateTime.Now}).\n";
            }
        }

        public bool TryRestoreBackup()
        {
            var result = _fileManager.RestoreBackupForFile(DefaultPaths.DefaultTargetAssemblyPath);

            RestoreBackupCommand.RaiseCanExecuteChanged();

            return result;
        }

        private List<bool> TryLoadLastAppState()
        {
            var lastState = new List<bool> { true, false, false, true, false };

            try
            {
                lastState = _stateManager.LoadState();
            }
            catch
            {
                Status += "Can't load last state";
            }

            return lastState;
        }

        private void ApplyState(List<bool> state)
        {
            if (state.Count < 1) return;
            EnableDebugConsole = state[0];

            if (state.Count < 2) return;
            ShowMissingElementColorInfos = state[1];

            if (state.Count < 3) return;
            ShowMissingTypeColorOffsets = state[2];

            if (state.Count < 4) return;
            SkipTiles = state[3];

            if (state.Count < 5) return;
            ShowWhite = state[4];
        }
    }
}
