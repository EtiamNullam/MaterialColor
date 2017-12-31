using Core.IO;
using System;

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

        // TODO: log exception
        public static void EnterToggleSprite(KIconToggleMenu.ToggleInfo toggleInfo, KIconToggleMenu toggleMenu)
        {
            try
            {
                // TODO: remove magic string
                if (toggleInfo.icon == "overlay_materialcolor")
                {
                    toggleInfo.toggle.fgImage.sprite = FileManager.LoadSpriteFromFile(Common.Paths.MaterialColorOverlayIconPath, 256, 256);
                }
            }
            catch (Exception e) { }
        }
    }
}
