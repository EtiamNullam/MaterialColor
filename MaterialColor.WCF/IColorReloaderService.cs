using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MaterialColor.WCF
{
    [ServiceContract]
    public interface IColorReloaderService
    {
        [OperationContract]
        void ReloadColors();
    }
}
