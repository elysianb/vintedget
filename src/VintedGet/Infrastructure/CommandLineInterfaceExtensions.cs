using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VintedGet.Infrastructure
{
    public static class CommandLineInterfaceExtensions
    {
        public static bool HasOnlyOneArgument(this string[] args)
        {
            return args.Length == 1 && !args[0].StartsWith("-");
        }

        public static bool HasParameters(this string[] args)
        {
            return args.Any(x => x.StartsWith("-"));
        }

        public static bool HasParameter(this string[] args, string key, string shortKey = null)
        {
            if (string.IsNullOrEmpty(shortKey))
            {
                return args.Any(x => x == "--" + key);
            }

            return args.Any(x => x == "--" + key) || args.Any(x => x == "-" + shortKey);
        }

        public static string GetParameterValue(this string[] args, string key, string shortKey, string defaultValue = null)
        {
            string result = defaultValue;

            if (HasParameter(args, key, shortKey))
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--" + key || args[i] == "-" + shortKey)
                    {
                        if (args.Length - 1 > i)
                        {
                            result = args[i + 1];
                        }

                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(result) || result.StartsWith("--") || result.EndsWith("-"))
            {
                return defaultValue;
            }

            return result;
        }
    }
}
