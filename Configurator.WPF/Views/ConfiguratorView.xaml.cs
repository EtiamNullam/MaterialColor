using System;
using System.Windows;
using System.Windows.Controls;

namespace Configurator.WPF.Views
{
    public partial class ConfiguratorView : Window
    {
        public ConfiguratorView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                var logger = new Common.IO.Logger(Common.Paths.ConfiguratorLogFileName);

                logger.Log("ConfiguratorView init failed.");
                logger.Log(e);

                Application.Current.Shutdown(2);
            }
        }

        private void OnStatusChanged(object sender, TextChangedEventArgs e)
        {
            var statusTextBox = sender as TextBox;
            statusTextBox?.ScrollToEnd();
        }
    }
}
