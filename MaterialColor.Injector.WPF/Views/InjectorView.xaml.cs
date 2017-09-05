using System.Windows;
using System.Windows.Controls;

namespace MaterialColor.Injector.WPF.Views
{
    public partial class InjectorView : Window
    {
        public InjectorView()
        {
            InitializeComponent();
        }

        private void OnStatusChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox statusTextBox)
            {
                statusTextBox.ScrollToEnd();
            }
        }
    }
}
