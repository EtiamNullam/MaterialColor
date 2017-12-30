using System;
using System.Windows;
using System.Windows.Controls;

namespace Injector.WPF.Views
{
    public partial class InjectorView : Window
    {
        public InjectorView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                var logger = new Common.IO.Logger(Common.Paths.InjectorLogFileName);

                logger.Log("InjectorView init failed.");
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
