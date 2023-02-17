using Tizen.NUI;

namespace SettingCore
{
    public static class ColorExtension
    {
        public static string ToHex(this Color color)
        {
            static string toHex(float f)
            {
                int i = (int)(f * 255);
                return string.Format($"{i:X2}");
            }
            string r = toHex(color.R);
            string g = toHex(color.G);
            string b = toHex(color.B);
            string a = color.A == 1 ? "" : toHex(color.A);
            return $"#{r}{g}{b}{a}";
        }
    }
}
