﻿using System.Windows;
using System.Windows.Controls;

namespace Configurator.Views
{
    public partial class ConfiguratorView : Window
    {
        public ConfiguratorView()
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
