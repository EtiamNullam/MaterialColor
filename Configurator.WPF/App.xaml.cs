using System.Windows;

namespace Configurator.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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
