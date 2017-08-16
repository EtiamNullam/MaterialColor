using System.Windows;
using System.Linq;
using System;

namespace Injector.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length != 0)
            {
                foreach (var argument in e.Args.Select(arg => arg.ToLower()).ToList())
                {
                    if (argument == "-recover" || argument == "-r")
                    {
                        new FileManager().RestoreBackupForFile(DefaultPaths.DefaultTargetAssemblyPath);
                        Shutdown();
                        return;
                    }
                }
            }

            new Bootstrapper().Run();
        }
    }
}
