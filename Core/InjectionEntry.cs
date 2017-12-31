namespace Core
{
    public static class InjectionEntry
    {
        public static void LogicWireBridgeEnter(BuildingDef logicWireBridgeDef)
        {
            logicWireBridgeDef.ObjectLayer = ObjectLayer.Backwall;
        }
    }
}
