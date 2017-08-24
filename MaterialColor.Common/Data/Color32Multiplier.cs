namespace MaterialColor.Common.Data
{
    public class Color32Multiplier
    {
        [Newtonsoft.Json.JsonConstructor]
        public Color32Multiplier(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public Color32Multiplier(float all)
        {
            Red = Green = Blue = all;
        }

        public static readonly Color32Multiplier One = new Color32Multiplier(1);

        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }
    }
}
