namespace ShitForum.Mappers
{
    public class Options
    {
        public Options(bool sage, bool noNoko)
        {
            Sage = sage;
            NoNoko = noNoko;
        }

        public bool Sage { get; }
        public bool NoNoko { get; }
    }
}