using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;
using System.IO;

namespace Sandbox
{
    internal class GlobalSettings
    {
        private static GlobalSettings _instance = new GlobalSettings();
        public static GlobalSettings Instance => _instance;

        public string Authority => "https://www.vinted.fr";
        public string UserAgent => "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36";
        public string AcceptLanguage => "fr-FR,fr;q=0.9,en-US;q=0.8,en;q=0.7";
    }
    
    internal class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            //{
            //    Console.WriteLine("usage: sandbox user_id");
            //    return;
            //}

            //var userId = args[0];
            var userId = "11871779";

            TestConnection(userId);
        }

        static void TestConnection(string userId)
        {
            using (var client = new WebClient())
            {
                //foreach (var header in GetHeaders())
                //{
                //    client.Headers.Add(header.Key, header.Value);
                //    Console.WriteLine($"{header.Key}  {header.Value}");
                //}

                var url = $"https://www.vinted.fr/api/v2/users/{userId}?localize=false";
                Console.WriteLine(url);

                var cookies = File.ReadAllText("Headers.formatted.txt").Replace(System.Environment.NewLine, "; ");
                client.Headers.Add(HttpRequestHeader.Cookie, cookies);
                client.Headers.Add("Accept-Encoding", $"identity");
                client.Headers.Add("Accept-Language", GlobalSettings.Instance.AcceptLanguage);
                client.Headers.Add("User-Agent", GlobalSettings.Instance.UserAgent);
                var json = client.DownloadString(url);

                Console.WriteLine(json);
            }
        }

        static Dictionary<string, string> GetHeaders()
        {
            var headers = new Dictionary<string, string>();
            var fileContent = File.ReadAllLines("Headers.txt");
            var key = string.Empty;
            var value = string.Empty;
            foreach (var line in fileContent)
            {
                if (line.EndsWith(":"))
                {
                    key = line.TrimEnd(':');
                }
                else
                {
                    value = line;
                    headers[key] = value;
                }
            }

            //for (int i = 0; i < headersString.Length; i++)
            //{
            //    if (!headersString[i].StartsWith(":"))
            //    {
            //        var headerValue = headersString[i + 1];
            //        var headerName = string.Join("-", headersString[i].TrimEnd(':')
            //            .Split('-')
            //            .Select(word => (word.Length > 0 ? char.ToUpper(word[0]).ToString() : "") + (word.Length > 1 ? word.Substring(1).ToLower() : "")));

            //        if (!headers.ContainsKey(headerName))
            //        {
            //            headers.Add(headerName, headerValue);
            //        }
            //    }

            //    i++;
            //}

            return headers;
        }
    }
}