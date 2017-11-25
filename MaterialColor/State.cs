using Common;
using Common.Data;
using Common.Json;
using MaterialColor.IO;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialColor
{
    public static class State
    {
        private static JsonFileLoader _jsonLoader = new JsonFileLoader(new JsonManager(), Logger);

        public static Common.IO.Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new Common.IO.Logger(Paths.MaterialCoreLogFileName);
                }

                return _logger;
            }
        }

        private static Common.IO.Logger _logger;

        public static Dictionary<string, Color32> TypeColorOffsets
        {
            get
            {
                if (_typeColorOffsets == null)
                {
                    _jsonLoader.TryLoadTypeColorOffsets(out var colorOffsets);
                    TypeColorOffsets = colorOffsets;
                }

                return _typeColorOffsets;
            }
            private set
            {
                _typeColorOffsets = value;
            }
        }

        private static Dictionary<string, Color32> _typeColorOffsets = null;

        public static Dictionary<SimHashes, ElementColorInfo> ElementColorInfos
        {
            get
            {
                if (_elementColorInfos == null)
                {
                    if (_jsonLoader.TryLoadElementColorInfos(out var colorInfos))
                    {
                        ElementColorInfos = colorInfos;
                    }
                    else
                    {
                        ElementColorInfos = new Dictionary<SimHashes, ElementColorInfo>();
                    }
                }

                return _elementColorInfos;
            }
            private set
            {
                _elementColorInfos = value;
            }
        }

        private static Dictionary<SimHashes, ElementColorInfo> _elementColorInfos = null;

        public static MaterialColorState ConfiguratorState
        {
            get
            {
                if (_configuratorState == null)
                {
                    _jsonLoader.TryLoadConfiguratorState(out var state);
                    ConfiguratorState = state;
                }

                return _configuratorState;
            }
            private set
            {
                _configuratorState = value;
            }
        }

        private static MaterialColorState _configuratorState = null;

        public static bool TryReloadConfiguratorState()
        {
            if (_jsonLoader.TryLoadConfiguratorState(out var state))
            {
                ConfiguratorState = state;
                return true;
            }

            return false;
        }

        public static bool TryReloadTypeColorOffsets()
        {
            if (_jsonLoader.TryLoadTypeColorOffsets(out var colorOffsets))
            {
                TypeColorOffsets = colorOffsets;
                return true;
            }

            return false;
        }

        public static bool TryReloadElementColorInfos()
        {
            if (_jsonLoader.TryLoadElementColorInfos(out var colorInfos))
            {
                ElementColorInfos = colorInfos;
                return true;
            }

            return false;
        }

        // TODO: load from file instead
        public static readonly List<string> TileNames = new List<string>
        {
            "Tile", "MeshTile", "InsulationTile", "GasPermeableMembrane", "TilePOI", "PlasticTile", "MetalTile"
        };
    }
}
