using MaterialColor.Extensions;
using System;
using UnityEngine;

namespace MaterialColor.Helpers
{
    public static class ColorHelper
    {
        public static void UpdateBuildingColor(BuildingComplete building)
        {
            var buildingName = building.name.Replace("Complete", string.Empty);
            var material = MaterialHelper.ExtractMaterial(building);

            Color32 color;

            if (State.ConfiguratorState.Enabled)
            {
                switch (State.ConfiguratorState.ColorMode)
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

            if (State.TileNames.Contains(buildingName))
            {
                try
                {
                    if (TileColors == null)
                    {
                        TileColors = new Color?[Grid.CellCount];
                    }

                    TileColors[Grid.PosToCell(building.gameObject)] = color;

                    return;
                }
                catch (Exception e)
                {
                    State.Logger.Log("Error while aquiring cell color");
                    State.Logger.Log(e);
                }
            }

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
                        else // anything else
                        {
                            var kAnimControllerBase = building.GetComponent<KAnimControllerBase>();

                            if (kAnimControllerBase != null)
                            {
                                kAnimControllerBase.TintColour = color;
                            }
                            else
                            {
                                Debug.LogError($"Can't find KAnimControllerBase component in <{buildingName}> and its not a registered tile.");
                            }
                        }
                    }
                }
            }
        }

        public static Color?[] TileColors;

        public static readonly Color32 DefaultColor =
            new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

        public static readonly Color32 MissingDebugColor =
            new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);

        public static readonly Color32 NoOffset =
            new Color32(0, 0, 0, byte.MaxValue);

        private static void SetFilteredStorageColors(FilteredStorage storage, Color32 color, Color32 dimmedColor)
        {
            storage.filterTint = color;
            storage.noFilterTint = dimmedColor;
            storage.FilterChanged();
        }

        public static bool TryGetTypeStandardColor(string typeName, out Color32 standardColor)
        {
            Color32 typeStandardColor;
            if (State.TypeColorOffsets.TryGetValue(typeName, out typeStandardColor))
            {
                standardColor = typeStandardColor;
                return true;
            }

            standardColor = State.ConfiguratorState.ShowMissingTypeColorOffsets
                ? MissingDebugColor
                : NoOffset;

            return false;
        }

        public static Color DefaultCellColor
            => new Color(1, 1, 1);

        public static Color InvalidCellColor
            => new Color(1, 0, 0);

        private static void BreakdownGridObjectsComponents(int cellIndex)
        {
            for (int i = 0; i <= 20; i++)
            {
                State.Logger.Log("Starting object from grid component breakdown, index: " + cellIndex);

                try
                {
                    var comps = Grid.Objects[cellIndex, i].GetComponents<Component>();

                    foreach (var comp in comps)
                    {
                        State.Logger.Log($"Object Layer: {i}, Name: {comp.name}, Type: {comp.GetType()}");
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    State.Logger.Log($"Cell Index: {cellIndex}, Layer: {i}");
                    State.Logger.Log(e);
                }
                //catch { }
            }
        }

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

        public static Color GetCellOverlayColor(int cellIndex)
        {
            var cell = Grid.Cell[cellIndex];
            var element = ElementLoader.elements[cell.elementIdx];
            var substance = element.substance;

            var overlayColor = substance.overlayColour;

            overlayColor.a = byte.MaxValue;

            return overlayColor;
        }
    }
}
