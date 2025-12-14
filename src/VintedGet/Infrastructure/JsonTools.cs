using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VintedGet.Domain.Model;

namespace VintedGet.Infrastructure
{
    internal class JsonTools
    {
        public static string StripTags(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return Regex.Replace(input, "<.*?>", string.Empty, RegexOptions.Singleline);
        }

        public static string NormalizeWhitespace(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return Regex.Replace(s, @"\s+", " ").Trim();
        }

        public static T DeserializeJson<T>(string json)
        {
            var instance = typeof(T);

            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var deserializer = new DataContractJsonSerializer(instance);
                return (T)deserializer.ReadObject(ms);
            }
        }

        public static string SerializeJson<T>(T obj)
        {
            if (obj == null)
                return null;

            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(ms, obj);
                ms.Position = 0;

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    string json = reader.ReadToEnd();
                    // Replace escaped slashes with normal slashes
                    json = json.Replace(@"\/", "/");
                    return json;
                }
            }
        }

        public static string ExtractJsonObject(string html, string marker, char startToken, char endToken)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            int startIndex = html.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (startIndex < 0)
                return null;

            startIndex += marker.Length - 1; // position at first [

            int bracketCount = 0;
            int endIndex = startIndex;
            bool started = false;

            while (endIndex < html.Length)
            {
                char c = html[endIndex];

                if (c == startToken) { bracketCount++; started = true; }
                else if (c == endToken) bracketCount--;

                endIndex++;

                if (started && bracketCount == 0) break;
            }

            if (bracketCount != 0)
                return null; // unbalanced brackets

            // Extract JSON array substring
            string jsonArrayEscaped = html.Substring(startIndex, endIndex - startIndex);

            // Unescape backslashes
            string jsonArray = jsonArrayEscaped.Replace("\\\"", "\"");

            return jsonArray;
        }

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
