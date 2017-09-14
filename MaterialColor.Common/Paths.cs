using System.IO;

namespace MaterialColor.Common
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

        public const string ElementColorInfosFileName = "ElementColorInfos.json";
        public const string TypeColorsFileName = "TypeColorOffsets.json";
        public const string InjectorStateFileName = "InjectorState.json";
        public const string MaterialColorStateFileName = "MaterialColorState.json";

        public static readonly string ElementColorInfosPath = MaterialConfigPath + Path.DirectorySeparatorChar + ElementColorInfosFileName;
        public static readonly string TypeColorsPath = MaterialConfigPath + Path.DirectorySeparatorChar + TypeColorsFileName;
        public static readonly string InjectorStatePath = MaterialConfigPath + Path.DirectorySeparatorChar + InjectorStateFileName;
        public static readonly string MaterialColorStatePath = MaterialConfigPath + Path.DirectorySeparatorChar + MaterialColorStateFileName;

        public const string MaterialCoreLogFileName = "MaterialCoreLog.txt";
        public const string ConfiguratorLogFileName = "ConfiguratorLog.txt";
        public const string InjectorLogFileName = "InjectorLog.txt";
        public const string OnionLogFileName = "OnionLog.txt";
    }
}
