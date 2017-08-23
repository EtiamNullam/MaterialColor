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

                var color = State.Disabled
                    ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)
                    : material.GetMaterialColorForType(buildingName);

                var dimmedColor = color.SetBrightness(color.GetBrightness() / 2);

                // storagelocker
                var storageLocker = building.GetComponent<StorageLocker>();

                if (storageLocker != null)
                {
                    storageLocker.filterTint = color;
                    storageLocker.noFilterTint = dimmedColor;
                    storageLocker.OnFilterChanged(storageLocker.filterable.GetTags());
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
                            rationBox.filterTint = color;
                            rationBox.noFilterTint = dimmedColor;
                            rationBox.OnFilterChanged(rationBox.filterable.GetTags());
                        }
                        else // refrigerator
                        {
                            var fridge = building.GetComponent<Refrigerator>();

                            if (fridge != null)
                            {
                                fridge.filterTint = color;
                                fridge.noFilterTint = dimmedColor;
                                fridge.OnFilterChanged(fridge.filterable.GetTags());
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

        public static Color32 GetTypeStandardColor(string typeName)
        {
            if (!State.TypeColorOffsets.TryGetValue(typeName, out Color32 typeStandardColor))
            {
                if (State.ConfiguratorState.ShowMissingTypeColorOffsets)
                {
                    Debug.LogError($"Can't find <{typeName}> type color");
                    return new Color32(0xFF, 0, 0xFF, 0xFF);
                }
                return new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            }
            return typeStandardColor;
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

            debugColor.a = 1;

            return debugColor;
        }
    }
}
