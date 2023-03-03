namespace SettingCore
{
    internal static class StringExtension
    {
        public static bool EqualsIgnoreCase(this string s, string compare)
        {
            return s.ToLowerInvariant().Equals(compare.ToLowerInvariant());
        }

        public static bool StartsWithIgnoreCase(this string s, string compare)
        {
            return s.ToLowerInvariant().StartsWith(compare.ToLowerInvariant());
        }
    }
}
