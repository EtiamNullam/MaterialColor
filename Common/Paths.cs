using System.IO;

namespace Common
{
    public static class Paths
    {
        public const string ModsDirectory = "Mods";

        public static readonly string LogsPath = ModsDirectory + Path.DirectorySeparatorChar + "Logs";

        public static readonly string OnionMainPath = ModsDirectory + Path.DirectorySeparatorChar + "OnionPatcher";
        public static readonly string OnionConfigPath = OnionMainPath + Path.DirectorySeparatorChar + "Config";

        public const string OnionStateFileName = "OnionState.json";

        public static readonly string OnionStatePath = OnionConfigPath + Path.DirectorySeparatorChar + OnionStateFileName;

        public static readonly string MaterialMainPath = ModsDirectory + Path.DirectorySeparatorChar + "MaterialColor";
        public static readonly string MaterialConfigPath = MaterialMainPath + Path.DirectorySeparatorChar + "Config";

        public const string DefaultElementColorInfosFileName = "0-default.json";
        public const string DefaultTypeColorOffsetsFileName = "0-default.json";
        public const string InjectorStateFileName = "InjectorState.json";
        public const string MaterialColorStateFileName = "MaterialColorState.json";

        public static readonly string ElementColorInfosDirectory = MaterialConfigPath + Path.DirectorySeparatorChar + "ElementColorInfos";
        public static readonly string TypeColorOffsetsDirectory = MaterialConfigPath + Path.DirectorySeparatorChar + "TypeColorOffsets";

        public static readonly string DefaultElementColorInfosPath = ElementColorInfosDirectory + Path.DirectorySeparatorChar + DefaultElementColorInfosFileName;
        public static readonly string DefaultTypeColorOffsetsPath = TypeColorOffsetsDirectory + Path.DirectorySeparatorChar + DefaultTypeColorOffsetsFileName;
        public static readonly string InjectorStatePath = MaterialConfigPath + Path.DirectorySeparatorChar + InjectorStateFileName;
        public static readonly string MaterialColorStatePath = MaterialConfigPath + Path.DirectorySeparatorChar + MaterialColorStateFileName;

        public const string MaterialCoreLogFileName = "MaterialCoreLog.txt";
        public const string ConfiguratorLogFileName = "ConfiguratorLog.txt";
        public const string InjectorLogFileName = "InjectorLog.txt";
        public const string OnionLogFileName = "OnionLog.txt";
        public const string CommonLogFileName = "CommonLog.txt";

        public static readonly string SpritesPath = ModsDirectory + Path.DirectorySeparatorChar + "Sprites";

        public const string MaterialColorOverlayIconFileName = "overlay_materialColor.png";

        public static readonly string MaterialColorOverlayIconPath = SpritesPath + Path.DirectorySeparatorChar + MaterialColorOverlayIconFileName;

    }
}
