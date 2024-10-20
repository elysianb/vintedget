using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace VintedGet.Infrastructure
{
    public class JwtToken
    {
        public string Token { get; }
        public string UserId { get; }

        public JwtToken(string token)
        {
            Token = token;

            // Split the JWT into parts
            string[] parts = token.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid JWT token format");
            }

            Console.WriteLine("Token  : " + token);

            // Decode header
            string header = Base64UrlDecode(parts[0]);
            Console.WriteLine("Header : " + header);

            // Decode payload
            string payload = Base64UrlDecode(parts[1]);
            Console.WriteLine("Payload: " + payload);

            // Deserialize JSON parts (Optional)
            var serializer = new JavaScriptSerializer();
            //var headerData = serializer.Deserialize<Dictionary<string, object>>(header);
            var payloadData = serializer.Deserialize<Dictionary<string, object>>(payload);
            UserId = payloadData["sub"].ToString();
        }

        // Helper function to Base64 URL-decode JWT parts
        private static string Base64UrlDecode(string input)
        {
            // Convert Base64Url to Base64
            string base64 = input.Replace('-', '+').Replace('_', '/');

            // Add padding if needed
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            // Decode Base64 string to bytes
            byte[] bytes = Convert.FromBase64String(base64);

            // Convert bytes to UTF8 string
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
