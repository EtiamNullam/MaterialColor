using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaterialColor
{
    public class State
    {
        public static Dictionary<string, Color32> TypeColorOffsets = new Dictionary<string, Color32>();
        public static Dictionary<SimHashes, ElementColorInfo> ElementColorInfos = new Dictionary<SimHashes, ElementColorInfo>();

        public static ConfiguratorState ConfiguratorState = new ConfiguratorState();

        public static readonly List<string> TileNames = new List<string>
        {
            "Tile", "MeshTile", "InsulationTile", "GasPermeableMembrane"
        };
    }
}
