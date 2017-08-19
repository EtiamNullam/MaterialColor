namespace MaterialColor.Core.Helpers
{
    public static class MaterialHelper
    {
        public static SimHashes ExtractMaterial(KAnimControllerBase kAnimController)
            => kAnimController.GetComponent<PrimaryElement>().ElementID;
    }
}