using MaterialColor.Extensions;
using UnityEngine;

namespace MaterialColor.Helpers
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
                var color = material.GetMaterialColorForType(buildingName);
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
    }
}
