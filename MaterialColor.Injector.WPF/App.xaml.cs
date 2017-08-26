using System.Windows;
using System.Linq;
using System;

namespace MaterialColor.Injector.WPF
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
                        var fileManager = new IO.FileManager();

                        fileManager.RestoreBackupForFile(IO.Paths.DefaultAssemblyCSharpPath);
                        fileManager.RestoreBackupForFile(IO.Paths.DefaultAssemblyFirstPassPath);
                        Shutdown(2);
                        return;
                    }
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
    }
}
