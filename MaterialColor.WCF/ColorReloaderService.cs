using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MaterialColor.WCF
{
    public class ColorReloaderService : IColorReloaderService
    {
        public void ReloadColors()
        {
            OnColorsReload?.Invoke(this, EventArgs.Empty);
        }

        public static event EventHandler OnColorsReload;
    }
}
