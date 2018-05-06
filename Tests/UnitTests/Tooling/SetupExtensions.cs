using System.Threading.Tasks;
using Moq.Language;
using Moq.Language.Flow;

namespace UnitTests.Tooling
{
    public static class SetupExtensions
    {
        public static IReturnsResult<TMock> ReturnsT<TMock, TResult>(this IReturns<TMock, Task<TResult>> t, TResult o) where TMock : class => 
            t.Returns(Task.FromResult(o));
    }
}
