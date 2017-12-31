using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data;
using Common.Json;
using UnityEngine;

namespace JsonSorter.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                return;
            }
            if (args.Length <= 2)
            {
                _filePath = args[1];
            }

            var mode = _elementArgAliases.Contains(args[0].ToLower())
                ? Mode.Element
                : _typeArgAliases.Contains(args[0].ToLower())
                    ? Mode.Type
                    : Mode.None;

            if (mode == Mode.Element)
            {
                try
                {
                    SortElementColorInfos();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"ElementColorInfos sorting failed.\n{e.Message}\n{e.StackTrace}");
                }
            }

            if (mode == Mode.Type)
            {
                try
                {
                    SortTypeColorOffsets();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"TypeColorOffsets sorting failed.\n{e.Message}\n{e.StackTrace}");
                }
            }
        }

        private static string _filePath;
        private static readonly JsonManager _jsonManager = new JsonManager();
        private static readonly ElementColorInfosManager _elementColorInfosManager = new ElementColorInfosManager(_jsonManager);
        private static readonly TypeColorOffsetsManager _typeColorOffsetsManager = new TypeColorOffsetsManager(_jsonManager);

        private static readonly List<string> _typeArgAliases = new List<string>
        {
            "-type", "-t"
        };

        private static readonly List<string> _elementArgAliases = new List<string>
        {
            "-element", "-e"
        };

        private static void SortElementColorInfos()
        {
            var elementColorInfos = _elementColorInfosManager.LoadSingleElementColorInfosFile(_filePath).ToList();

            elementColorInfos.Sort(CompareElementColorInfoDictionaryPairs);

            _elementColorInfosManager.SaveElementsColorInfo(elementColorInfos.ToDictionary(element => element.Key, element => element.Value), _filePath);
        }

        private static void SortTypeColorOffsets()
        {
            var typeColorOffsets = _typeColorOffsetsManager.LoadSingleTypeColorOffsetsFile(_filePath).ToList();

            typeColorOffsets.Sort(CompareTypeColorOffsetDictionaryPairs);

            _typeColorOffsetsManager.SaveTypesColors(typeColorOffsets.ToDictionary(element => element.Key, element => element.Value), _filePath);
        }

        private static int CompareElementColorInfoDictionaryPairs(KeyValuePair<SimHashes, ElementColorInfo> a, KeyValuePair<SimHashes, ElementColorInfo> b)
            => string.Compare(a.Key.ToString(), b.Key.ToString(), StringComparison.Ordinal);

        private static int CompareTypeColorOffsetDictionaryPairs(KeyValuePair<string, Color32> a, KeyValuePair<string, Color32> b)
            => string.Compare(a.Key, b.Key, StringComparison.Ordinal);
    }
}
