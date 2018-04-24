using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Services.Dtos;

namespace ShitForum.Hasher
{
    public class TripCodeHasher
    {
        private readonly Guid salt;

        private static IEnumerable<string> Break(string s, string breakOn) => s.Split(breakOn, 2);

        private readonly IReadOnlyDictionary<string, Func<string, string>> rules;

        public TripCodeHasher(IConfiguration config)
        {
            this.salt = config.GetSection("TripCodeSalt").Get<Guid>();
            this.rules = new Dictionary<string, Func<string, string>>()
            {
                {"##", s =>
                    {
                        var broken = Break(s, "##").ToArray();
                        return broken[0] + "!!" + Sha256Hasher.Hash(salt + broken[1]);
                    }
                },
                {
                    "#",
                    s =>
                    {
                        var broken = Break(s, "#").ToArray();
                        return broken[0] + "!" + Sha256Hasher.Hash(broken[1]);
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
