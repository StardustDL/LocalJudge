using System;
using System.Collections.Generic;
using System.Linq;

namespace StarOJ.Core.Helpers
{
    public class Command
    {
        public string Name { get; set; }

        public string[] Arguments { get; set; } = Array.Empty<string>();

        public Command Resolve(Dictionary<string, string> variables = null)
        {
            string resolve(string raw, Dictionary<string, string> vars)
            {
                if (variables != null && variables.TryGetValue(raw, out string value) == true)
                {
                    return value;
                }
                else
                {
                    return raw;
                }
            }
            return new Command
            {
                Name = resolve(Name, variables),
                Arguments = Arguments.Select(x => resolve(x, variables)).ToArray()
            };
        }
    }
}
