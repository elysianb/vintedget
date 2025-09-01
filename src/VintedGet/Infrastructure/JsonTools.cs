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
            var startToken1 = "self.__next_f.push(";
            var endToken1 = ")";
            var scriptContent = javascript.Substring(startToken1.Length);
            scriptContent = scriptContent.Substring(0, scriptContent.Length - 1);
            var jsonValue = System.Json.JsonValue.Parse(scriptContent);
            var rawValue = jsonValue[1].ToString();
            int colonIndex = rawValue.IndexOf(':');
            rawValue = rawValue.Substring(colonIndex + 1);
            rawValue = rawValue.Substring(0, rawValue.Length - 1);
            rawValue = rawValue.Replace("\\\\\\\"", "'").Replace("\\\"", "\"").Replace("\\n", "").Replace("\\", "");

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
