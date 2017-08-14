using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using UnityEngine;
using Common;
using Common.Data;
using MaterialColor.Extensions;

namespace MaterialColor.Helpers
{
    public static class MaterialHelper
    {
        // TODO: move
        public static SimHashes ExtractMaterial(KAnimControllerBase kAnimController)
            => kAnimController.GetComponent<PrimaryElement>().ElementID;

    }
}