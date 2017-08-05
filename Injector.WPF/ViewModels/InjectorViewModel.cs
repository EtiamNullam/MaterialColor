using Injector.WPF.DAL;
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
        public InjectorViewModel(JsonManagerProxy jsonManager, FileManager fileManager, DefaultInjector injector)
        {
            PatchCommand = new DelegateCommand(Patch);
            RestoreBackupCommand = new DelegateCommand(RestoreBackup, CanRestoreBackup);

            _jsonManager = jsonManager;
            _fileManager = fileManager;
            _injector = injector;

            EnableDebugConsole = TryLoadLastAppState();
        }

        ~InjectorViewModel()
        {
            _jsonManager.SaveState(EnableDebugConsole);
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

        private JsonManagerProxy _jsonManager;
        private FileManager _fileManager;
        private DefaultInjector _injector;

        private string _status;
        private bool _enableDebugConsole;

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
                    resultInfo.Append("\tBackup Failed.\n\tPatch cancelled.\n");
                    return;
                }
            }

            _injector.InjectDefaultAndBackup(EnableDebugConsole);

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

        private bool TryLoadLastAppState()
        {
            var lastEnableDebugConsole = false;

            try
            {
                lastEnableDebugConsole = _jsonManager.LoadLastState();
            }
            catch
            {
                Status += "Can't load last state";
            }

            return lastEnableDebugConsole;
        }

    }
}
