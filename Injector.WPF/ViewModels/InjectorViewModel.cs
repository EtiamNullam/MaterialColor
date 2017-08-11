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
    // TODO: only save state after patch
    public class InjectorViewModel : BindableBase
    {
        public InjectorViewModel(InjectorStateManager stateManager, FileManager fileManager, DefaultInjector injector)
        {
            PatchCommand = new DelegateCommand(Patch);
            RestoreBackupCommand = new DelegateCommand(RestoreBackup, CanRestoreBackup);

            _stateManager = stateManager;
            _fileManager = fileManager;
            _injector = injector;

            TryLoadLastAppState();
        }

        public DelegateCommand PatchCommand { get; private set; }
        public DelegateCommand RestoreBackupCommand { get; private set; }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public bool EnableDebugConsole
        {
            get => _enableDebugConsole;
            set => SetProperty(ref _enableDebugConsole, value);
        }

        private InjectorStateManager _stateManager;
        private FileManager _fileManager;
        private DefaultInjector _injector;

        private string _status;
        private bool _enableDebugConsole;

        public bool CanRestoreBackup()
            => _fileManager.BackupForFileExists(DefaultPaths.DefaultTargetAssemblyPath);

        public void Patch()
        {
            //StringBuilder resultInfo = new StringBuilder($"Patch result [{DateTime.Now.TimeOfDay}]:\n");
            Status += $"Patching started [{DateTime.Now.TimeOfDay}]\n";

            if (CanRestoreBackup())
            {
                if (TryRestoreBackup())
                {
                    //resultInfo.Append("\tBackup restored.\n");
                    Status += "\tBackup restored.\n";
                }
                else
                {
                    //resultInfo.Append("\tBackup failed.\n\tPatch cancelled.\n");
                    Status += "\tBackup failed.\n\tPatch cancelled.\n";
                    return;
                }
            }

            try
            {
                _injector.InjectDefaultAndBackup(EnableDebugConsole);
            }
            catch (Exception e)
            {
                //resultInfo.Append("\tInjection failed.\n" + e.Message + "\n" + e.StackTrace);
                //Status += resultInfo.ToString();
                Status += "\tInjection failed.\n" + e.Message + "\n" + e.StackTrace + "\n";
                return;
            }

            RestoreBackupCommand.RaiseCanExecuteChanged();

            //resultInfo.Append("\tOriginal backed up.\n\tOriginal patched.\n\tPatch successful.\n");
            Status += "\tOriginal backed up.\n\tOriginal patched.\n\tPatch successful.\n";

            //Status += resultInfo.ToString();

            try
            {
                Common.IOHelper.EnsureDirectoryExists(Common.DefaultPaths.Directory);
            }
            catch (Exception e)
            {
                Status += "Can't create or access directory for state to save.\n" + e.Message + "\n" + e.StackTrace + "\n";
                //resultInfo.Append("Can't create or access directory for state to save.\n" + e.Message + "\n" + e.StackTrace);
                //Status += resultInfo.ToString();
                return;
            }

            try
            {
                _stateManager.SaveState(EnableDebugConsole);
            }
            catch (Exception e)
            {
                Status += "Can't save app state.\n" + e.Message + "\n" + e.StackTrace + "\n";
                //resultInfo.Append("Can't save app state.\n" + e.Message + "\n" + e.StackTrace);
                //Status += resultInfo.ToString();
                return;
            }
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

        private void TryLoadLastAppState()
        {
            try
            {
                EnableDebugConsole = _stateManager.LoadState();
            }
            catch
            {
                Status += "Can't load last state";
            }
        }
    }
}
