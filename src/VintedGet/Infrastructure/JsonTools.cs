using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VintedGet.Infrastructure
{
    internal class JsonTools
    {
        public static JsonValue GetFromNextJSHydration(string javascript)
        {
            var startToken = "self.__next_f.push(";
            var endToken = ")";
            var scriptContent = javascript.Substring(startToken.Length);
            scriptContent = scriptContent.Substring(0, scriptContent.Length - endToken.Length);

            var jsonValue = System.Json.JsonValue.Parse(scriptContent);
            var rawValue = jsonValue[1].ToString();
            int colonIndex = rawValue.IndexOf(':');
            rawValue = rawValue.Substring(colonIndex + 1);
            rawValue = rawValue.Substring(0, rawValue.Length - 1);
            rawValue = rawValue
                .Replace("\\\\\\\"", "'") // replace double quotes to simple quotes in string property values
                .Replace("\\\"", "\"") // replace double quotes for json properties name and value delimiters
                .Replace("\\n", "") // remove the newline at the end of the rawValue, and eventually other newlines
                .Replace("\\", ""); // remove all other remaining escape chars

            jsonValue = System.Json.JsonValue.Parse(rawValue);
            return jsonValue;
        }

        public static IEnumerable<JsonValue> FindByProperty(JsonValue value, string propertyName)
        {
            if (value is JsonObject obj)
            {
                foreach (var kvp in obj)
                {
                    if (kvp.Key == propertyName)
                        yield return kvp.Value;

                    foreach (var inner in FindByProperty(kvp.Value, propertyName))
                        yield return inner;
                }
            }
            else if (value is JsonArray arr)
            {
                foreach (var element in arr)
                {
                    foreach (var inner in FindByProperty(element, propertyName))
                        yield return inner;
                }
            }
        }
    }
}
