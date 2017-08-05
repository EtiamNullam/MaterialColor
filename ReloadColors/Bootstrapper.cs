using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReloadColors
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override void InitializeShell()
        {
            App.Current.MainWindow.Show();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.TryResolve<Views.ReloadColorsView>();
        }
    }
}
