using System.Net;

namespace Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("usage: sandbox user_id");
                return;
            }

            TestConnection(args[0]);
        }

        static void TestConnection(string userId)
        {
            using (var client = new WebClient())
            {
                foreach (var header in GetHeaders())
                {
                    client.Headers.Add(header.Key, header.Value);
                    Console.WriteLine($"{header.Key}  {header.Value}");
                }

                var url = $"https://www.vinted.fr/api/v2/users/{userId}?localize=false";
                Console.WriteLine(url);

                var json = client.DownloadString(url);

                Console.WriteLine(json);
            }
        }

        static Dictionary<string, string> GetHeaders()
        {
            var headers = new Dictionary<string, string>();
            var headersString = File.ReadAllLines("Headers.txt");
            for(int i = 0; i < headersString.Length; i++)
            {
                if (!headersString[i].StartsWith(":"))
                {
                    var headerValue = headersString[i+1];
                    var headerName = string.Join("-", headersString[i].TrimEnd(':')
                        .Split('-')
                        .Select(word => (word.Length > 0? char.ToUpper(word[0]) : "") + (word.Length > 1 ? word.Substring(1).ToLower() : "")));

                    headers.Add(headerName, headerValue);
                }

                i++;
            }

            return headers;
        }
    }
}