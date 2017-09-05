using System.Windows;
using System.Linq;
using System;
using System.Collections.Generic;
using MaterialColor.Common.Json;

namespace MaterialColor.Injector.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IO.FileManager FileManager
        {
            get
            {
                if (_fileManager == null)
                {
                    _fileManager = new IO.FileManager();
                }

                return _fileManager;
            }
        }

        private IO.FileManager _fileManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length != 0)
            {
                bool inject = false;
                bool recover = false;
                bool enableDebugConsole = false;

                foreach (var argument in e.Args.Select(arg => arg.ToLower()).ToList())
                {
                    if (InjectArgumentAliases.Contains(argument))
                    {
                        recover = inject = true;
                    }
                    else if (RecoverArgumentAliases.Contains(argument))
                    {
                        recover = true;
                    }
                    else if (EnableDebugArgumentAliases.Contains(argument))
                    {
                        enableDebugConsole = true;
                    }
                }

                if (inject || recover)
                {
                    Recover();

                    if (inject)
                    {
                        Inject(enableDebugConsole);
                        new InjectorStateManager(new JsonManager()).SaveState(enableDebugConsole);
                    }

                    Shutdown();

                    return;
                }
            }

            try
            {
                new Bootstrapper().Run();
            }
            catch
            {
                Shutdown(1);
            }
        }

        private void Recover()
        {
            var fileManager = new IO.FileManager();

            fileManager.RestoreBackupForFile(IO.Paths.DefaultAssemblyCSharpPath);
            fileManager.RestoreBackupForFile(IO.Paths.DefaultAssemblyFirstPassPath);
        }

        private void Inject(bool enableDebugConsole)
        {
            new InjectionManager(FileManager).InjectDefaultAndBackup(enableDebugConsole);
        }

        private List<string> RecoverArgumentAliases = new List<string> { "-r", "-recover" };
        private List<string> InjectArgumentAliases = new List<string> { "-i", "-inject" };
        private List<string> EnableDebugArgumentAliases = new List<string> { "-d", "-enabledebug" };
    }
}
