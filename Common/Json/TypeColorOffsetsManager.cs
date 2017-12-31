using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Common.Json
{
    public class TypeColorOffsetsManager : BaseManager
    {
        public TypeColorOffsetsManager(JsonManager manager, IO.Logger logger = null) : base(manager, logger) { }

        public void SaveTypesColors(Dictionary<string, Color32> dictionary, string path = null)
        {
            if (path == null)
            {
                path = Paths.DefaultTypeColorOffsetsPath;
            }

            _manager.Serialize(dictionary, path);
        }

        public Dictionary<string, Color32> LoadTypeColorOffsetsDirectory(string directoryPath = null)
        {
            if (directoryPath == null)
            {
                directoryPath = Paths.TypeColorOffsetsDirectory;
            }

            var directory = new DirectoryInfo(directoryPath);
            var files = directory.GetFiles("*.json");

            Dictionary<string, Color32> result = new Dictionary<string, Color32>();

            foreach (var file in files)
            {
                var filePath = Path.Combine(directoryPath, file.Name);
                Dictionary<string, Color32> resultFromCurrentFile;

                try
                {
                    resultFromCurrentFile = LoadSingleTypeColorOffsetsFile(filePath);
                }
                catch (Exception e)
                {
                    if (_logger != null)
                    {
                        _logger.Log($"Error loading {filePath} as TypeColorOffset configuration file.");
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

                _logger?.Log($"Loaded {filePath} as TypeColorOffset configuration file.");
            }

            return result;
        }

        public Dictionary<string, Color32> LoadSingleTypeColorOffsetsFile(string path)
        {
            return _manager.Deserialize<Dictionary<string, Color32>>(path);
        }
    }
}
