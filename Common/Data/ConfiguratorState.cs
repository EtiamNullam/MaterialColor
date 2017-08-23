namespace MaterialColor.Common.Data
{
    public class ConfiguratorState
    {
        public bool ShowMissingElementColorInfos { get; set; }
        public bool ShowMissingTypeColorOffsets { get; set; }
        public bool ShowBuildingsAsWhite { get; set; }
        public bool ShowDetailedErrorInfo { get; set; }

        public ColorMode ColorMode { get; set; } = ColorMode.Json;
    }
}
