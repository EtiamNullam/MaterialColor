using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MaterialColor.Common.Data;
using UnityEngine;
using MaterialColor.Common.Json;

namespace JsonSorter.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 2)
            {
                _filePath = args[1];
            }
            else if (args.Length <= 0)
            {
                return;
            }

            var mode = args[0] == "element" ? Mode.Element : args[0] == "type" ? Mode.Type : Mode.None;

            if (mode == Mode.None) return;

            if (mode == Mode.Element)
            {
                try
                {
                    SortElementColorInfos();
                    return;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"ElementColorInfos sorting failed.\n{e.Message}\n{e.StackTrace}");
                    return;
                }
            }

            if (mode == Mode.Type)
            {
                try
                {
                    SortTypeColorOffsets();
                    return;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"TypeColorOffsets sorting failed.\n{e.Message}\n{e.StackTrace}");
                    return;
                }
            }
        }

        private static string _filePath = null;

        private static void SortElementColorInfos()
        {
            var manager = new ElementColorInfosManager();
            var elementColorInfos = manager.LoadElementColorInfos(_filePath).ToList();

            elementColorInfos.Sort(new Comparison<KeyValuePair<SimHashes, ElementColorInfo>>(CompareElementColorInfoDictionaryPairs));

            manager.SaveElementsColorInfo(elementColorInfos.ToDictionary(element => element.Key, element => element.Value), _filePath);
        }

        private static void SortTypeColorOffsets()
        {
            var manager = new TypeColorOffsetsManager();
            var typeColorOffsets = manager.LoadTypeColorOffsets(_filePath).ToList();

            typeColorOffsets.Sort(new Comparison<KeyValuePair<string, Color32>>(CompareTypeColorOffsetDictionaryPairs));

            manager.SaveTypesColors(typeColorOffsets.ToDictionary(element => element.Key, element => element.Value), _filePath);
        }

        private static int CompareElementColorInfoDictionaryPairs(KeyValuePair<SimHashes, ElementColorInfo> a, KeyValuePair<SimHashes, ElementColorInfo> b)
            => a.Key.ToString().CompareTo(b.Key.ToString());

        private static int CompareTypeColorOffsetDictionaryPairs(KeyValuePair<string, Color32> a, KeyValuePair<string, Color32> b)
            => a.Key.CompareTo(b.Key);
    }
}
