using MaterialColor.Common.Json;
using MaterialColor.Injector.IO;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace MaterialColor.Injector.WPF.ViewModels
{
    public class InjectorViewModel : BindableBase
    {
        public InjectorViewModel(InjectorStateManager stateManager, FileManager fileManager, InjectionManager injector)
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
            set => SetProperty(ref _status, $"{_status}\n{value}".Trim());
        }

        public bool EnableDebugConsole
        {
            get => _enableDebugConsole;
            set => SetProperty(ref _enableDebugConsole, value);
        }

        private InjectorStateManager _stateManager;
        private FileManager _fileManager;
        private InjectionManager _injector;

        private string _status;
        private bool _enableDebugConsole;

        public bool CanRestoreBackup()
            => _fileManager.BackupForFileExists(Paths.DefaultAssemblyCSharpPath) | _fileManager.BackupForFileExists(Paths.DefaultAssemblyFirstPassPath);

        public void Patch()
        {
            Status = $"[{DateTime.Now.TimeOfDay}] Patching started.";

            if (CanRestoreBackup())
            {
                if (TryRestoreBackup())
                {
                    Status = "\tBackup restored.";
                }
                else
                {
                    Status = "\tBackup failed.\n\tPatch cancelled.";
                    return;
                }
            }

            try
            {
                _injector.InjectDefaultAndBackup(EnableDebugConsole);
            }
            catch (Exception e)
            {
                Status = "\tInjection failed.\n" + e.Message + "\n" + e.StackTrace;
                return;
            }

            RestoreBackupCommand.RaiseCanExecuteChanged();

            Status = "\tOriginal backed up.\n\tOriginal patched.\n\tPatch successful.";

            try
            {
                Common.IOHelper.EnsureDirectoryExists(Common.Paths.Directory);
            }
            catch (Exception e)
            {
                Status = "Can't create or access directory for state to save.\n" + e.Message + "\n" + e.StackTrace;
                return;
            }

            try
            {
                _stateManager.SaveState(EnableDebugConsole);
            }
            catch (Exception e)
            {
                Status = "Can't save app state.\n" + e.Message + "\n" + e.StackTrace;
                return;
            }
        }

        public void RestoreBackup()
        {
            if (TryRestoreBackup())
            {
                Status = $"[{DateTime.Now.TimeOfDay}] Backup restore successful.";
            }
            else
            {
                Status = $"[{DateTime.Now.TimeOfDay}]Backup restore failed.";
            }
        }

        public bool TryRestoreBackup()
        {
            bool result = false;

            try
            {
                result = _fileManager.RestoreBackupForFile(Paths.DefaultAssemblyCSharpPath)
                    | _fileManager.RestoreBackupForFile(Paths.DefaultAssemblyFirstPassPath);
            }
            catch (Exception e)
            {
                Status = $"Can't restore backup.\n{e.Message}\n{e.StackTrace}";
                result = false;
            }

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
                Status = "Can't load last state.";
            }
        }
    }
}
