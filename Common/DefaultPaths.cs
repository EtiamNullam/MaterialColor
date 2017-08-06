using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class DefaultPaths
    {
        public static readonly string ElementColorInfosPath = Directory + Path.DirectorySeparatorChar + ElementColorInfosFilePath;
        public static readonly string TypeColorsPath = Directory + Path.DirectorySeparatorChar + TypeColorsFilePath;
        public static readonly string InjectorStatePath = Directory + Path.DirectorySeparatorChar + InjectorStateFilePath;

        public const string Directory = "MaterialColor";

        public const string ElementColorInfosFilePath = "ElementColorInfos.json";
        public const string TypeColorsFilePath = "TypeColorOffsets.json";
        public const string InjectorStateFilePath = "InjectorState.json";
    }
}
