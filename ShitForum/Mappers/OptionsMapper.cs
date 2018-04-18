namespace ShitForum.Mappers
{
    public static class OptionsMapper
    {
        private static bool SimpleContains(string needle, string haystack) =>
            haystack != null && haystack.ToLowerInvariant().Contains(needle.ToLowerInvariant());

        public static Options Map(string s) => new Options(SimpleContains("sage", s), SimpleContains("nonoko", s));
    }
}