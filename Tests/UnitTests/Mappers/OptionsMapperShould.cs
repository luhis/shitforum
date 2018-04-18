using FluentAssertions;
using ShitForum.Mappers;
using Xunit;

namespace UnitTests.Mappers
{
    public class OptionsMapperShould
    {
        [Fact]
        public void FindNothing()
        {
            var res = OptionsMapper.Map("dsajasjaa");
            res.Sage.Should().BeFalse();
            res.NoNoko.Should().BeFalse();
        }


        [Fact]
        public void FindSAGE()
        {
            var res = OptionsMapper.Map("aaaaSAGEaaa");
            res.Sage.Should().BeTrue();
            res.NoNoko.Should().BeFalse();
        }

        [Fact]
        public void FindNoko()
        {
            var res = OptionsMapper.Map("qqqnonokoqqq");
            res.Sage.Should().BeFalse();
            res.NoNoko.Should().BeTrue();
        }
    }
}