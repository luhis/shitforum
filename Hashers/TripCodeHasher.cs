using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Services.Dtos;

namespace Hashers
{
    public class TripCodeHasher
    {
        private readonly Guid salt;

        private static string[] Break(string s, string breakOn) => s.Split(breakOn.ToCharArray(), 2);

        private readonly IReadOnlyDictionary<string, Func<string, string>> rules;

        public TripCodeHasher(IConfiguration config)
        {
            this.salt = config.GetSection("TripCodeSalt").Get<Guid>();
            this.rules = new Dictionary<string, Func<string, string>>()
            {
                {"##", s =>
                    {
                        var broken = Break(s, "##");
                        return broken[0] + "!!" + Repeater.DoXTimes(salt + broken[1], Sha256Hasher.Hash, 10);
                    }
                },
                {
                    "#",
                    s =>
                    {
                        var broken = Break(s, "#");
                        return broken[0] + "!" + Repeater.DoXTimes(broken[1], Sha256Hasher.Hash, 10);
                    }
                },
                {
                    string.Empty,
                    s => s
                }
            };
        }

        public TripCodedName Hash(string s) => new TripCodedName(rules.First(f => s.Contains(f.Key)).Value(s));
    }
}
