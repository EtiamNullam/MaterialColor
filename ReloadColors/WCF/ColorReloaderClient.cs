using MaterialColor.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ReloadColors.WCF
{
    // not tested
    public class ColorReloaderClient
    {
        public ColorReloaderClient()
        {
            _factory = new ChannelFactory<IColorReloaderService>(
                new NetNamedPipeBinding(NetNamedPipeSecurityMode.None),
                new EndpointAddress(Common.DefaultPaths.ReloadColorsAddress));

            _proxy = _factory.CreateChannel();
        }

        ~ColorReloaderClient()
        {
            _factory?.Close();

            _proxy = null;
            _factory = null;
        }

        private ChannelFactory<IColorReloaderService> _factory;

        private IColorReloaderService _proxy;

        public void SendReload()
        {
            _proxy.ReloadColors();
        }
    }
}
