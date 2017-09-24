using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class OverlayMenuManager
    {
        // TODO: read from file instead
        public static void OnOverlayMenuPrefabInit(OverlayMenu overlayMenu)
        {
            overlayMenu.overlay_toggle_infos.Add(
                new OverlayMenu.OverlayToggleInfo(
                    "Toggle MaterialColor",
                    "overlay_materialcolor",
                    (SimViewMode) Common.IDs.ToggleMaterialColorOverlayID,
                    string.Empty,
                    (Action) Common.IDs.ToggleMaterialColorOverlayAction,
                    "Toggles MaterialColor overlay",
                    "MaterialColor"));
        }
    }
}
