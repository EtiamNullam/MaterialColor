﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
