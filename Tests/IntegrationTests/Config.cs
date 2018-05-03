using System.Linq;
using Domain.Repositories;
using FluentAssertions;
using Persistence;
using Persistence.Repositories;
using Xunit;

namespace IntegrationTests
{

    public class Config : IShitForumDbConfig
    {
        public Config(string dbLocation)
        {
            DbLocation = dbLocation;
        }

        public string DbLocation { get; }
    }
}
