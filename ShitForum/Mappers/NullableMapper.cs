using Optional;

namespace ShitForum.Mappers
{
    public static class NullableMapper
    {
        public static Option<T> ToOption<T>(T t) where T : class => t == null ? Option.None<T>() : Option.Some(t);
    }
}