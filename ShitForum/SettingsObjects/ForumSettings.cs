using Microsoft.Extensions.Configuration;

namespace ShitForum.SettingsObjects
{
    public class ForumSettings
    {
        public ForumSettings(IConfiguration conf)
        {
            this.ForumName = conf.GetSection("ForumName").Get<string>();
        }

        public string ForumName { get; }
    }
}
