using FluentAssertions;
using Services;
using Services.Dtos;
using ShitForum.Hasher;
using Xunit;

namespace UnitTests
{
    public class TripCodeHasherShould
    {
        public TripCodeHasherShould()
        {
            var conf = MockConfig.Get();
            this.hasher = new TripCodeHasher(conf);
        }

        private readonly TripCodeHasher hasher;

        [Fact]
        public void NotCrash()
        {
            hasher.Hash("aaa");
        }

        [Fact]
        public void AcceptNoCode()
        {
            var res = hasher.Hash("Matt M");
            res.Should().BeEquivalentTo(new TripCodedName("Matt M"));
        }

        [Fact]
        public void HashTheCode()
        {
            var res = hasher.Hash("Matt M #abcdefg");
            res.Should().BeEquivalentTo(new TripCodedName("Matt M !fRpUEnsiJQL1t5tfsIAwYRUqRPkrN+I8ZSe69mXU2po="));
        }

        [Fact]
        public void SecureHashTheCode()
        {
            var res = hasher.Hash("Matt M ##abcdefg");
            res.Should().BeEquivalentTo(new TripCodedName("Matt M !!cHbVOC7c5jnZ6q3fxa7jUnx4ZzgqslO+L47lrFPEhPY="));
        }
    }
}
