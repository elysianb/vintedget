using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using VintedGet.Domain.Model;

namespace VintedGet.Infrastructure
{
    public class ItemBuilder
    {
        public static PageProperties Extract(string itemId, string httpBody, string output)
        {
            return ItemBuilder_December2025.Extract(itemId, httpBody, output);
        }
    }

    public class ItemBuilder_December2025
    {
        public static PageProperties Extract(string itemId, string httpBody, string output)
        {
            var itemTitle = ExtractItemTitleFromHtml(httpBody);
            var itemBrand = ExtractItemBrandFromHtml(httpBody);
            var itemSize = ExtractItemSizeFromHtml(httpBody);
            var itemPhotos = ExtractPhotosFromHtml(httpBody).ToArray();

            var sellerId = ExtractSellerIdFromHtml(httpBody);
            var sellerUsername = ExtractSellerUsernameFromHtml(httpBody);
            var sellerPhoto = ExtractSellerPhotoFromHtml(httpBody);

            var item = new PageProperties
            {
                ItemDto = new ItemDto
                {
                    Id = itemId,
                    Title = itemTitle,
                    BrandTitle = itemBrand,
                    Size = itemSize,
                    Url = $"https://www.vinted.com/items/{itemId}",
                    Photos = itemPhotos,
                    User = new User
                    {
                        Id = sellerId,
                        Login = sellerUsername,
                        Photo = sellerPhoto
                    },
                    SellerPhoto = sellerPhoto
                }
            };

            var jsonString = JsonTools.SerializeJson(item);
            System.IO.File.WriteAllText(System.IO.Path.Combine(output, $"{itemId}.vget-response.json"), jsonString);

            return item;
        }

        public static string ExtractSellerIdFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            // Match \"sellerId\":\"<value>\"
            var match = Regex.Match(
                html,
                @"(?:""|\\+"")sellerId(?:""|\\+)"":\s*(?:""?)(\d+)(?:""?)",
                RegexOptions.IgnoreCase
            );

            return match.Success ? match.Groups[1].Value : null;
        }

        public static Photo ExtractSellerPhotoFromHtml(string html)
        {
            string jsonArray = JsonTools.ExtractJsonObject(html, "\\\"seller_photo\\\":{", '{', '}');

            if (string.IsNullOrEmpty(jsonArray))
                return null;

            // Deserialize JSON
            var dto = JsonTools.DeserializeJson<Photo>(jsonArray);

            return dto;
        }

        public static List<Photo> ExtractPhotosFromHtml(string html)
        {
            string jsonArray = JsonTools.ExtractJsonObject(html, "\\\"photos\\\":[", '[', ']');

            // Deserialize JSON
            var dtoList = JsonTools.DeserializeJson<List<Photo>>(jsonArray);

            return dtoList ?? new List<Photo>();
        }

        public static string ExtractItemTitleFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            var match = Regex.Match(
                html,
                @"<div\b[^>]*\bdata-testid\s*=\s*[""']item-page-summary-plugin[""'][^>]*>.*?<h1\b[^>]*>(.*?)</h1>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        public static string ExtractItemBrandFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            var match = Regex.Match(
                html,
                @"<span\b[^>]*\bitemprop\s*=\s*[""']name[""'][^>]*>(.*?)</span>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            return match.Success ? match.Groups[1].Value : null;
        }

        public static string ExtractItemSizeFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            var match = Regex.Match(
                html,
                @"<div\b[^>]*\bitemprop\s*=\s*[""']size[""'][^>]*>.*?<span\b[^>]*>(.*?)</span>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            if (!match.Success)
                return null;

            // Remove all tags
            string textOnly = Regex.Replace(match.Groups[1].Value, "<.*?>", string.Empty);

            // Trim whitespace
            textOnly = textOnly.Trim();

            return textOnly;
        }

        public static string ExtractSellerImgFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            var match = Regex.Match(
                html,
                @"<img\b[^>]*\brole\s*=\s*[""']img[""'][^>]*\bsrc\s*=\s*[""']([^""']+)[""']",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            return match.Success ? match.Groups[1].Value : null;
        }

        public static string ExtractSellerUsernameFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            var match = Regex.Match(
                html,
                @"<span\b[^>]*\bdata-testid\s*=\s*[""']profile-username[""'][^>]*>(.*?)</span>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            return match.Success ? match.Groups[1].Value.Trim() : null;
        }
    }

    public class ItemBuilder_September2025
    {
        public static PageProperties Extract(string itemId, string httpBody, string output)
        {
            var dtoString = string.Empty;
            var pattern = @"<script>(.*?)<\/script>";
            MatchCollection matches = Regex.Matches(httpBody, pattern, RegexOptions.Singleline);
            foreach (Match match in matches)
            {
                string scriptContent = match.Groups[1].Value;

                if (scriptContent.Contains("item") && scriptContent.Contains("brand_dto"))
                {
                    var jsonValue = JsonTools.GetFromNextJSHydration(scriptContent);
                    var items = JsonTools.FindByProperty(jsonValue, "item");
                    var plugins = JsonTools.FindByProperty(jsonValue, "plugins");

                    System.IO.File.WriteAllText(System.IO.Path.Combine(output, $"{itemId}.vget-response.json"), scriptContent);
                    var itemObject = JsonTools.DeserializeJson<ItemDto>(items.First().ToString());
                    var pluginsObject = JsonTools.DeserializeJson<PluginDto[]>(plugins.Last().ToString());

                    return new PageProperties
                    {
                        ItemDto = itemObject,
                        Plugins = pluginsObject
                    };
                }
            }

            return null;
        }
    }
}
