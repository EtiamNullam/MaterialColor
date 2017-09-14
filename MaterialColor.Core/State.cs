using MaterialColor.Common.Data;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialColor.Core
{
    public static class State
    {
        public static Dictionary<string, Color32> TypeColorOffsets = new Dictionary<string, Color32>();
        public static Dictionary<SimHashes, ElementColorInfo> ElementColorInfos = new Dictionary<SimHashes, ElementColorInfo>();

        public static MaterialColorState ConfiguratorState = new MaterialColorState();

        public static bool Disabled = false;

        public static readonly List<string> TileNames = new List<string>
        {
            "Tile", "MeshTile", "InsulationTile", "GasPermeableMembrane", "TilePOI"
        };
    }
}
