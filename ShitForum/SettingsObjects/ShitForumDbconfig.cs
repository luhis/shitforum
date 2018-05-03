using Microsoft.Extensions.Configuration;
using Persistence;

namespace ShitForum.SettingsObjects
{
    public class ShitForumDbConfig : IShitForumDbConfig
    {
        public ShitForumDbConfig(IConfiguration conf)
        {
            this.DbLocation = conf.GetSection("DbPath").Get<string>();
        }

        public string DbLocation { get; }
    }
}
