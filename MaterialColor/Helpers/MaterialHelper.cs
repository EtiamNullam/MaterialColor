namespace MaterialColor.Helpers
{
    public static class MaterialHelper
    {
        public static SimHashes GetMaterialFromCell(int cellIndex)
        {
            if (!Grid.IsValidCell(cellIndex))
            {
                return SimHashes.Vacuum;
            }

            var cellElementIndex = Grid.Cell[cellIndex].elementIdx;
            var element = ElementLoader.elements[cellElementIndex];

            return element.id;
        }

        public static SimHashes ExtractMaterial(KAnimControllerBase kAnimController)
            => kAnimController.GetComponent<PrimaryElement>().ElementID;
    }
}