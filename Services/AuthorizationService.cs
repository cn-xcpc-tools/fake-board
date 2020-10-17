using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Board.Services
{
    public class AuthorizationService
    {
        private readonly Dictionary<string, string> dict;

        public AuthorizationService(IOptions<BoardOptions> options)
        {
            dict = (options.Value.Password ?? Array.Empty<string>())
                .Select(s => s.Split(new[] { ':' }, 2))
                .ToDictionary(s => s[0], s => s[1]);
        }

        public bool Authorize(Tuple<string, string> auth)
        {
            return dict.ContainsKey(auth.Item1)
                && dict[auth.Item1] == auth.Item2;
        }
    }
}
