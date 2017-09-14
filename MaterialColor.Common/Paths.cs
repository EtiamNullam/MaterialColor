using System.IO;

namespace MaterialColor.Common
{
    public static class Paths
    {
        public static readonly string ElementColorInfosPath = ConfigDirectory + Path.DirectorySeparatorChar + ElementColorInfosFileName;
        public static readonly string TypeColorsPath = ConfigDirectory + Path.DirectorySeparatorChar + TypeColorsFileName;
        public static readonly string InjectorStatePath = ConfigDirectory + Path.DirectorySeparatorChar + InjectorStateFileName;
        public static readonly string MaterialColorStatePath = ConfigDirectory + Path.DirectorySeparatorChar + MaterialColorStateFileName;
        public static readonly string OnionStatePath = ConfigDirectory + Path.DirectorySeparatorChar + OnionStateFileName;

        public const string ConfigDirectory = "MaterialColorConfig";

        public const string ElementColorInfosFileName = "ElementColorInfos.json";
        public const string TypeColorsFileName = "TypeColorOffsets.json";
        public const string InjectorStateFileName = "InjectorState.json";
        public const string MaterialColorStateFileName = "MaterialColorState.json";
        public const string OnionStateFileName = "OnionState.json";

        public const string LogsDirectory = "Logs";

        public const string MaterialCoreLogFileName = "MaterialCoreLog.txt";
        public const string ConfiguratorLogFileName = "ConfiguratorLog.txt";
        public const string InjectorLogFileName = "InjectorLog.txt";
        public const string OnionLogFileName = "OnionLog.txt";
    }
}
