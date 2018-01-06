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
        private static readonly JsonFileLoader _jsonLoader = new JsonFileLoader(new JsonManager(), Logger);

        public static Common.IO.Logger Logger
        {
            get { return _logger ?? (_logger = new Common.IO.Logger(Paths.MaterialCoreLogFileName)); }
        }

        private static Common.IO.Logger _logger;

        public static Dictionary<string, Color32> TypeColorOffsets
        {
            get
            {
                if (_typeColorOffsets != null) return _typeColorOffsets;

                Dictionary<string, Color32> colorOffsets;
                _jsonLoader.TryLoadTypeColorOffsets(out colorOffsets);
                TypeColorOffsets = colorOffsets;

                return _typeColorOffsets;
            }
            private set
            {
                _typeColorOffsets = value;
            }
        }

        private static Dictionary<string, Color32> _typeColorOffsets;

        public static Dictionary<SimHashes, ElementColorInfo> ElementColorInfos
        {
            get
            {
                if (_elementColorInfos != null) return _elementColorInfos;

                Dictionary<SimHashes, ElementColorInfo> colorInfos;
                _jsonLoader.TryLoadElementColorInfos(out colorInfos);
                ElementColorInfos = colorInfos;

                return _elementColorInfos;
            }
            private set
            {
                _elementColorInfos = value;
            }
        }

        private static Dictionary<SimHashes, ElementColorInfo> _elementColorInfos;

        public static MaterialColorState ConfiguratorState
        {
            get
            {
                if (_configuratorState != null) return _configuratorState;

                MaterialColorState state;
                _jsonLoader.TryLoadConfiguratorState(out state);
                ConfiguratorState = state;

                return _configuratorState;
            }
            private set
            {
                _configuratorState = value;
            }
        }

        private static MaterialColorState _configuratorState;

        public static bool TryReloadConfiguratorState()
        {
            MaterialColorState state;
            if (!_jsonLoader.TryLoadConfiguratorState(out state)) return false;
            ConfiguratorState = state;

            return true;
        }

        public static bool TryReloadTypeColorOffsets()
        {
            Dictionary<string, Color32> colorOffsets;
            if (!_jsonLoader.TryLoadTypeColorOffsets(out colorOffsets)) return false;
            TypeColorOffsets = colorOffsets;

            return true;
        }

        public static bool TryReloadElementColorInfos()
        {
            Dictionary<SimHashes, ElementColorInfo> colorInfos;
            if (!_jsonLoader.TryLoadElementColorInfos(out colorInfos)) return false;
            ElementColorInfos = colorInfos;

            return true;
        }

        // TODO: load from file instead
        public static readonly List<string> TileNames = new List<string>
        {
            "Tile", "MeshTile", "InsulationTile", "GasPermeableMembrane", "TilePOI", "PlasticTile", "MetalTile"
        };

        public static TemperatureOverlayState TemperatureOverlayState
        {
            get
            {
                if (_temperatureOvelayState != null) return _temperatureOvelayState;

                TemperatureOverlayState state;
                _jsonLoader.TryLoadTemperatureState(out state);
                TemperatureOverlayState = state;

                return _temperatureOvelayState;
            }
            private set
            {
                _temperatureOvelayState = value;
            }
        }

        public static List<float> DefaultTemperatures = new List<float>();

        public static List<Color> DefaultTemperatureColors = new List<Color>();

        private static TemperatureOverlayState _temperatureOvelayState;

        public static bool TryReloadTemperatureState()
        {
            TemperatureOverlayState temperatureState;
            if (!_jsonLoader.TryLoadTemperatureState(out temperatureState)) return false;
            TemperatureOverlayState = temperatureState;

            return true;
        }
    }
}
