using MaterialColor.Common.IO;
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
            _logger = new Logger(Common.Paths.InjectorLogFileName);

            PatchCommand = new DelegateCommand(Patch);
            RestoreBackupCommand = new DelegateCommand(RestoreBackup, CanRestoreBackup);
            ExitCommand = new DelegateCommand(App.Current.Shutdown);

            _stateManager = stateManager;
            _fileManager = fileManager;
            _injector = injector;

            TryLoadLastAppState();

            if (!IsCSharpPatched && CanRestoreCSharpBackup())
            {
                Status = "Warning: A backup for Assembly-CSharp.dll exists, but current assembly doesn't appear to be patched. Patching without restoring backup is advised.";
            }

            if (!IsFirstpassPatched && CanRestoreFirstpassBackup())
            {
                Status = "Warning: A backup for Assembly-CSharp-firstpass.dll exists, but current assembly doesn't appear to be patched. Patching without restoring backup is advised.";
            }
        }

        public DelegateCommand PatchCommand { get; private set; }
        public DelegateCommand RestoreBackupCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }

        public string Status
        {
            get => _status;
            set
            {
                SetProperty(ref _status, $"{_status}\n{value}".Trim());
                _logger.Log(value);
            }
        }

        public bool EnableDebugConsole
        {
            get => _enableDebugConsole;
            set => SetProperty(ref _enableDebugConsole, value);
        }

        private InjectorStateManager _stateManager;
        private FileManager _fileManager;
        private InjectionManager _injector;
        private Logger _logger;

        private string _status;
        private bool _enableDebugConsole;

        public bool CanRestoreBackup()
            => CanRestoreCSharpBackup() || CanRestoreFirstpassBackup();

        public bool CanRestoreCSharpBackup()
            => _fileManager.BackupForFileExists(Paths.DefaultAssemblyCSharpPath);

        public bool CanRestoreFirstpassBackup()
            => _fileManager.BackupForFileExists(Paths.DefaultAssemblyFirstPassPath);

        public bool IsCSharpPatched
            => _injector.IsCurrentAssemblyCSharpPatched();

        public bool IsFirstpassPatched
            => _injector.IsCurrentAssemblyFirstpassPatched();

        public void Patch()
        {
            Status = $"[{DateTime.Now.TimeOfDay}] Patching started.";

            if (CanRestoreCSharpBackup())
            {
                if (IsCSharpPatched)
                {
                    if (TryRestoreCSharpBackup())
                    {
                        Status = "\tAssembly-CSharp.dll backup restored.";
                    }
                    else
                    {
                        Status = "\tAssembly-CSharp.dll backup failed.\n\tPatch cancelled.";
                        return;
                    }
                }
                else
                {
                    Status = "\tAssembly-CSharp.dll backup restore SKIPPED.";
                }
            }

            if (CanRestoreFirstpassBackup())
            {
                if (IsFirstpassPatched)
                {
                    if (TryRestoreFirstpassBackup())
                    {
                        Status = "\tAssembly-CSharp-firstpass.dll backup restored.";
                    }
                    else
                    {
                        Status = "\tAssembly-CSharp-firstpass.dll backup failed.\n\tPatch cancelled.";
                        return;
                    }
                }
                else
                {
                    Status = "\tAssembly-CSharp-firstpass.dll backup restore SKIPPED.";
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
                Common.IO.IOHelper.EnsureDirectoryExists(Common.Paths.MaterialConfigPath);
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
            RestoreCSharpBackup();
            RestoreFirstpassBackup();
        }

        public void RestoreCSharpBackup()
        {
            if (TryRestoreCSharpBackup())
            {
                Status = $"[{DateTime.Now.TimeOfDay}] Assembly-CSharp.dll backup restore successful.";
            }
            else
            {
                Status = $"[{DateTime.Now.TimeOfDay}] Assembly-CSharp.dll backup restore failed.";
            }
        }

        public void RestoreFirstpassBackup()
        {
            if (TryRestoreFirstpassBackup())
            {
                Status = $"[{DateTime.Now.TimeOfDay}] Assembly-CSharp-firstpass.dll backup restore successful.";
            }
            else
            {
                Status = $"[{DateTime.Now.TimeOfDay}] Assembly-CSharp-firstpass.dll backup restore failed.";
            }
        }

        public bool TryRestoreCSharpBackup()
        {
            bool result = false;

            try
            {
                result = _fileManager.RestoreBackupForFile(Paths.DefaultAssemblyCSharpPath);
            }
            catch (Exception e)
            {

                Status = $"Can't restore Assembly-CSharp.dll backup.\n{e.Message}\n{e.StackTrace}";
                result = false;
            }

            RestoreBackupCommand.RaiseCanExecuteChanged();

            return result;
        }

        public bool TryRestoreFirstpassBackup()
        {
            bool result = false;

            try
            {
                result = _fileManager.RestoreBackupForFile(Paths.DefaultAssemblyFirstPassPath);
            }
            catch (Exception e)
            {

                Status = $"Can't restore Assembly-CSharp-firstpass.dll backup.\n{e.Message}\n{e.StackTrace}";
                result = false;
            }

            RestoreBackupCommand.RaiseCanExecuteChanged();

            return result;
        }

        [Obsolete]
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
