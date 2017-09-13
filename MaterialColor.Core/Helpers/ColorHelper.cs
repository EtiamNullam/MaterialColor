using MaterialColor.Core.Extensions;
using UnityEngine;

namespace MaterialColor.Core.Helpers
{
    public static class ColorHelper
    {
        public static void UpdateBuildingColor(BuildingComplete building)
        {
            var buildingName = building.name.Replace("Complete", string.Empty);
            var kAnimControllerBase = building.GetComponent<KAnimControllerBase>();

            if (kAnimControllerBase != null)
            {
                var material = MaterialHelper.ExtractMaterial(kAnimControllerBase);

                Color32 color;

                if (!State.Disabled)
                {
                    switch (State.ConfiguratorState.Material.ColorMode)
                    {
                        case Common.Data.ColorMode.Json:
                            color = material.GetMaterialColorForType(buildingName);
                            break;
                        case Common.Data.ColorMode.DebugColor:
                            color = material.ToDebugColor();
                            break;
                        case Common.Data.ColorMode.None:
                        default:
                            color = DefaultColor;
                            break;
                    }
                }
                else color = DefaultColor;

                var dimmedColor = color.SetBrightness(color.GetBrightness() / 2);

                // storagelocker
                var storageLocker = building.GetComponent<StorageLocker>();

                if (storageLocker != null)
                {
                    SetFilteredStorageColors(storageLocker.filteredStorage, color, dimmedColor);
                }
                else // ownable
                {
                    var ownable = building.GetComponent<Ownable>();

                    if (ownable != null)
                    {
                        ownable.ownedTint = color;
                        ownable.unownedTint = dimmedColor;
                        ownable.UpdateTint();
                    }
                    else // rationbox
                    {
                        var rationBox = building.GetComponent<RationBox>();

                        if (rationBox != null)
                        {
                            SetFilteredStorageColors(rationBox.filteredStorage, color, dimmedColor);
                        }
                        else // refrigerator
                        {
                            var fridge = building.GetComponent<Refrigerator>();

                            if (fridge != null)
                            {
                                SetFilteredStorageColors(fridge.filteredStorage, color, dimmedColor);
                            }
                            else
                            {
                                kAnimControllerBase.TintColour = color;
                            }
                        }
                    }
                }
            }
            else if (!State.TileNames.Contains(buildingName))
            {
                Debug.LogError($"Can't find KAnimControllerBase component in <{buildingName}>.");
            }
        }

        public readonly static Color32 DefaultColor =
            new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

        public readonly static Color32 MissingDebugColor =
            new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);

        public readonly static Color32 NoOffset =
            new Color32(0, 0, 0, byte.MaxValue);

        private static void SetFilteredStorageColors(FilteredStorage storage, Color32 color, Color32 dimmedColor)
        {
            storage.filterTint = color;
            storage.noFilterTint = dimmedColor;
            storage.FilterChanged();
        }

        public static bool TryGetTypeStandardColor(string typeName, out Color32 standardColor)
        {
            if (State.TypeColorOffsets.TryGetValue(typeName, out Color32 typeStandardColor))
            {
                standardColor = typeStandardColor;
                return true;
            }
            else
            {
                standardColor = State.ConfiguratorState.Material.ShowMissingTypeColorOffsets
                    ? MissingDebugColor
                    : NoOffset;

                return false;
            }
        }

        public static Color GetDefaultCellColor()
            => new Color(1, 1, 1);

        public static Color GetCellColorJson(int cellIndex)
        {
            var material = MaterialHelper.GetMaterialFromCell(cellIndex);
            return material.ToCellMaterialColor();
        }

        public static Color GetCellColorDebug(int cellIndex)
        {
            var cell = Grid.Cell[cellIndex];
            var element = ElementLoader.elements[cell.elementIdx];
            var substance = element.substance;

            var debugColor = substance.debugColour;

            debugColor.a = byte.MaxValue;

            return debugColor;
        }
    }
}
