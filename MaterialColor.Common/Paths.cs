using System.IO;

namespace MaterialColor.Common
{
    public static class Paths
    {
        public static readonly string ElementColorInfosPath = ConfigDirectory + Path.DirectorySeparatorChar + ElementColorInfosFilePath;
        public static readonly string TypeColorsPath = ConfigDirectory + Path.DirectorySeparatorChar + TypeColorsFilePath;
        public static readonly string InjectorStatePath = ConfigDirectory + Path.DirectorySeparatorChar + InjectorStateFilePath;
        public static readonly string MaterialColorStatePath = ConfigDirectory + Path.DirectorySeparatorChar + MaterialColorStateFilePath;
        public static readonly string OnionStatePath = ConfigDirectory + Path.DirectorySeparatorChar + OnionStateFilePath;

        public const string ConfigDirectory = "MaterialColorConfig";

        public const string ElementColorInfosFilePath = "ElementColorInfos.json";
        public const string TypeColorsFilePath = "TypeColorOffsets.json";
        public const string InjectorStateFilePath = "InjectorState.json";
        public const string MaterialColorStateFilePath = "MaterialColorState.json";
        public const string OnionStateFilePath = "OnionState.json";
    }
}
