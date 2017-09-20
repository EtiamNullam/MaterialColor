using System.Windows;
using System.Linq;
using System;
using System.Collections.Generic;
using Common.Json;
using Common.Data;

namespace Injector.WPF
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
                bool injectMaterial = false;
                bool injectOnion = false;
                bool recover = false;
                bool enableDebugConsole = false;

                foreach (var argument in e.Args.Select(arg => arg.ToLower()).ToList())
                {
                    if (InjectMaterialArgumentAliases.Contains(argument))
                    {
                        recover = injectMaterial = true;
                    }
                    else if (InjectOnionArgumentAliases.Contains(argument))
                    {
                        recover = injectOnion = true;
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

                if (injectMaterial || injectOnion || recover)
                {
                    Recover();

                    if (injectMaterial || injectOnion)
                    {
                        var state = new InjectorState
                        {
                            InjectMaterialColor = injectMaterial,
                            InjectOnion = injectOnion,
                            EnableDebugConsole = enableDebugConsole
                        };

                        Inject(state);

                        new InjectorStateManager(new JsonManager()).SaveState(state); 
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

        private void Inject(InjectorState state)
        {
            new InjectionManager(FileManager).InjectDefaultAndBackup(state);
        }

        private List<string> RecoverArgumentAliases = new List<string> { "-r", "-recover" };
        private List<string> InjectMaterialArgumentAliases = new List<string> { "-m", "-material" };
        private List<string> InjectOnionArgumentAliases = new List<string> { "-o", "-onion" };
        private List<string> EnableDebugArgumentAliases = new List<string> { "-d", "-enabledebug" };
    }
}
