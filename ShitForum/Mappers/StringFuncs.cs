namespace ShitForum.Mappers
{
    public static class StringFuncs
    {
        public static string MapString(string s, string defaultValue) => string.IsNullOrWhiteSpace(s) ? defaultValue : s;
    }
}