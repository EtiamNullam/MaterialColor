using Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using Common.IO;

namespace Common.Json
{
    public class ElementColorInfosManager : BaseManager
    {
        public ElementColorInfosManager(JsonManager manager, Logger logger = null) : base(manager, logger) { }

        public void SaveElementsColorInfo(Dictionary<SimHashes, ElementColorInfo> dictionary, string path = null)
        {
            if (path == null)
            {
                path = Paths.DefaultElementColorInfosPath;
            }

            _manager.Serialize(dictionary, path);
        }

        /// <summary>
        /// Loads ElementColorInfos assoctiated with material from the configuration files
        /// </summary>
        /// <returns></returns>
        public Dictionary<SimHashes, ElementColorInfo> LoadElementColorInfosDirectory(string directoryPath = null)
        {
            if (directoryPath == null)
            {
                directoryPath = Paths.ElementColorInfosDirectory;
            }

            var directory = new DirectoryInfo(directoryPath);
            var files = directory.GetFiles("*.json");

            Dictionary<SimHashes, ElementColorInfo> result = new Dictionary<SimHashes, ElementColorInfo>();

            foreach (var file in files)
            {
                var filePath = Path.Combine(directoryPath, file.Name);
                Dictionary<SimHashes, ElementColorInfo> resultFromCurrentFile;

                try
                {
                    resultFromCurrentFile = LoadSingleElementColorInfosFile(filePath);
                }
                catch (Exception e)
                {
                    if (_logger != null)
                    {
                        _logger.Log($"Error loading {filePath} as ElementColorInfo configuration file.");
                        _logger.Log(e);
                    }
                    continue;
                }

                foreach (var entry in resultFromCurrentFile)
                {
                    if (result.ContainsKey(entry.Key))
                    {
                        result[entry.Key] = entry.Value;
                    }
                    else
                    {
                        result.Add(entry.Key, entry.Value);
                    }
                }

                _logger?.Log($"Loaded {filePath} as ElementColorInfo configuration file.");
            }

            return result;
        }

        public Dictionary<SimHashes, ElementColorInfo> LoadSingleElementColorInfosFile(string filePath)
        {
            return _manager.Deserialize<Dictionary<SimHashes, ElementColorInfo>>(filePath);
        }
    }
}
