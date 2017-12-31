using System.Windows;
using System.Linq;
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

                foreach (var argument in e.Args.Select(arg => arg.ToLower()).ToList())
                {
                    if (_injectMaterialArgumentAliases.Contains(argument))
                    {
                        recover = injectMaterial = true;
                    }
                    else if (_injectOnionArgumentAliases.Contains(argument))
                    {
                        recover = injectOnion = true;
                    }
                    else if (_recoverArgumentAliases.Contains(argument))
                    {
                        recover = true;
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

        private readonly List<string> _recoverArgumentAliases = new List<string> { "-r", "-recover" };
        private readonly List<string> _injectMaterialArgumentAliases = new List<string> { "-m", "-material" };
        private readonly List<string> _injectOnionArgumentAliases = new List<string> { "-o", "-onion" };
    }
}
