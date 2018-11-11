using EnsureThat;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace ShitForum
{
    public class AdminSettings
    {
        private readonly IReadOnlyList<Tuple<string, Guid>> admins;

        public AdminSettings(IOptions<AdminSettingsRaw> conf)
        {
            EnsureArg.IsNotNull(conf, nameof(conf));
            admins = conf.Value.Gods.Select(a => Tuple.Create("admin", a)).ToList().AsReadOnly();
        }

        public Option<(string name, Guid key)> IsValid(Guid key)
        {
            var r = admins.SingleOrDefault(a => a.Item2 == key);
            return r == null ? Option.None<(string name, Guid key)>() : Option.Some((name: r.Item1, key: r.Item2));
        }
    }
}
