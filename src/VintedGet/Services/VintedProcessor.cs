using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VintedGet.Infrastructure;
using VintedGet.Domain.Model;
using System.Text.RegularExpressions;

namespace VintedGet.Services
{
    public class VintedSession
    {
        public string UserId { get; set; }
        public string Cookies { get; set; }
    }

    public class VintedProcessor
    {
        public static VintedSession GetSession()
        {
            string userId = null;
            string userCookies = null;

            if (
                System.IO.File.Exists(Path.Combine(GlobalSettings.Instance.SettingsFolder, ".v_uid"))
                && System.IO.File.Exists(Path.Combine(GlobalSettings.Instance.SettingsFolder, ".vinted_cookies"))
                )
            {
                userId = System.IO.File.ReadAllText(Path.Combine(GlobalSettings.Instance.SettingsFolder, ".v_uid"));
                userCookies = System.IO.File.ReadAllText(Path.Combine(GlobalSettings.Instance.SettingsFolder, ".vinted_cookies"));
            }

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userCookies))
            {
                Console.WriteLine("No user session found. Use vget --login to open a session. For more information, please read help.");
                return null;
            }

            return new VintedSession { UserId = userId, Cookies = userCookies };
        }

        //public static void EnsureHasSession(ref string userId, ref string userCookies)
        //{
        //    if (
        //        (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userCookies))
        //        && System.IO.File.Exists(Path.Combine(GlobalSettings.Instance.SettingsFolder, ".v_uid"))
        //        && System.IO.File.Exists(Path.Combine(GlobalSettings.Instance.SettingsFolder, ".vinted_cookies"))
        //        )
        //    {
        //        userId = System.IO.File.ReadAllText(Path.Combine(GlobalSettings.Instance.SettingsFolder, ".v_uid"));
        //        userCookies = System.IO.File.ReadAllText(Path.Combine(GlobalSettings.Instance.SettingsFolder, ".vinted_cookies"));
        //    }
        //}

        public static void Logout()
        {
            System.IO.Directory.CreateDirectory(GlobalSettings.Instance.SettingsFolder);
            System.IO.File.Delete(Path.Combine(GlobalSettings.Instance.SettingsFolder, ".v_uid"));
            System.IO.File.Delete(Path.Combine(GlobalSettings.Instance.SettingsFolder, ".vinted_cookies"));
            Console.WriteLine($"logged out");
        }

        //public static bool HasSession(params string[] args)
        //{
        //    if (args.Any(x => string.IsNullOrEmpty(x)))
        //    {
        //        Console.WriteLine("No user session found. Use vget --login to open a session. For more information, please read help.");
        //        return false;
        //    }

        //    return true;
        //}

        public static string GetItemIdFromUrl(string itemUrl)
        {
            return itemUrl.Split('/').Last().Split('-')[0];
        }

        public static string GetFileNameFromUrl(string url)
        {
            return url.Split('/').Last().Split('?').First();
        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        public static string CleanString(string s)
        {   
            if (string.IsNullOrEmpty(s))
            { 
                return s; 
            }

            return RemoveDiacritics(s.Replace("/", "").Replace(" ", "").Replace("'", ""));
        }

        public static string GetItemMetadata(Domain.Model.ItemDto item)
        {
            var brand = CleanString(item.Brand?.Title);
            var size = CleanString(item.Size);
            if (!string.IsNullOrEmpty(brand) && !string.IsNullOrEmpty(size))
            {
                return $"{item.Id}-{brand}-T{size}";
            }

            return $"{item.Id}";
        }

        public static string DownloadString(WebClient client, string filename, string url, int sleepDurationInMilliseconds, int maxRetry)
        {
            var success = false;
            var retryCount = 0;
            string json = null;

            do
            {
                try
                {
                    Console.WriteLine($"- Download from : {url}");
                    Console.WriteLine($"  To file       : {filename}");
                    if (retryCount > 0)
                    {
                        Console.WriteLine($"  Retry         : {retryCount}/{maxRetry}");
                    }

                    Thread.Sleep(sleepDurationInMilliseconds);
                    json = client.DownloadString(url);
                    System.IO.File.WriteAllText(filename, json);

                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Exception     : {ex.Message}");
                }
            } while (!success && retryCount++ < maxRetry);

            return json;
        }

        public static void DownloadFile(WebClient client, string url, string filename, int sleepDurationInMilliseconds, int maxRetry)
        {
            var success = false;
            var retryCount = 0;

            do
            {
                try
                {
                    Console.WriteLine($"- Download from : {url}");
                    Console.WriteLine($"  To file       : {filename}");
                    if (retryCount > 0)
                    {
                        Console.WriteLine($"  Retry         : {retryCount}/{maxRetry}");
                    }

                    Thread.Sleep(sleepDurationInMilliseconds);
                    client.DownloadFile(url, filename);
                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Exception     : {ex.Message}");
                }
            } while (!success && retryCount++ < maxRetry);
        }

        public static HttpClient AquirePublicSession(out string csrfToken)
        {
            var authority = GlobalSettings.Instance.Authority;

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler();
            handler.CookieContainer = cookieContainer;

            Console.WriteLine($"Acquire session from {authority}");
            var client = new HttpClient(handler);

            var response = client.GetAsync(authority).Result;

            var cookieList = CookieContainerExtentions.GetAllCookies(cookieContainer);
            Console.WriteLine("Cookies:");
            foreach (Cookie cookie in cookieList)
            {
                Console.WriteLine($"{cookie.Name}:{cookie.Value}");
            }

            var body = response.Content.ReadAsStringAsync().Result;
            var startBlocPattern = "<meta name=\"csrf-token\" content=\"";
            var endBlocPattern = "\"";
            if (body.IndexOf(startBlocPattern) == -1)
            {
                Console.WriteLine($"WARING: csrf-token meta not found on {authority}");
                csrfToken = null;
            }
            else
            {
                var startBloc = body.Substring(body.IndexOf(startBlocPattern) + startBlocPattern.Length);
                csrfToken = startBloc.Substring(0, startBloc.IndexOf(endBlocPattern));
                Console.WriteLine($"CSRF-Token:{csrfToken}");
            }

            return client;
        }

        public static User GetUser(string userId, string cookies)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Cookie, cookies);
                client.Headers.Add("Accept-Encoding", $"identity");
                client.Headers.Add("Accept-Language", GlobalSettings.Instance.AcceptLanguage);
                client.Headers.Add("User-Agent", GlobalSettings.Instance.UserAgent);

                var url = $"{GlobalSettings.Instance.Authority}/api/v2/users/{userId}?localize=false";
                var filename = $"{userId}-user.vget-response.json";
                var json = DownloadString(client, filename, url, 1000, 3);

                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }

                return DeserializeJson<Domain.Model.UserResponse>(json).User;
            }
        }

        public static void GetPhotos(HttpClient httpClient, string csrfToken, string url, string output = null)
        {
            if (output == null)
            {
                output = GlobalSettings.Instance.Output;
            }

            var logs = new List<string>();
            logs.Add("url=" + url);
            Console.WriteLine($"Article: {url}");
            var itemId = VintedProcessor.GetItemIdFromUrl(url);

            string[] articlePhotos;
            string profilePhoto;
            Domain.Model.ItemDto item = null;

            var uri = $"{GlobalSettings.Instance.Authority}/items/{itemId}";
            //var uri = $"https://www.vinted.fr/api/v2/items/{itemId}";

            //int retryCount = 0;
            //do
            //{
            //    Console.WriteLine($"Getting data (retry {retryCount})...");
            //    using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
            //    {
            //        Thread.Sleep(sleepDurationInMilliseconds);

            //        request.Headers.Add("Accept-Encoding", $"identity");
            //        request.Headers.Add("User-Agent", $"python-urllib3/1.26.13");
            //        request.Headers.Add("X-CSRF-Token", csrfToken);
            //        Console.WriteLine($"GET:{uri}");
            //        var httpResponse = httpClient.SendAsync(request).Result;
            //        logs.Add($"statusCode={httpResponse.StatusCode}");
            //        logs.Add($"reasonPhrase={httpResponse.ReasonPhrase}");
            //        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
            //        {
            //            Console.WriteLine("Not Found.");
            //            System.IO.File.WriteAllLines(System.IO.Path.Combine(output, $"{itemId}.vget-summary.log"), logs);
            //            return;
            //        }

            //        var json = httpResponse.Content.ReadAsStringAsync().Result;
            //        System.IO.File.WriteAllText(System.IO.Path.Combine(output, $"{itemId}.vget-response.json"), json);

            //        var response = VintedProcessor.DeserializeJson<Domain.ItemResponse>(json);

            //        item = response.Item;
            //    }

            //} while (item == null && ++retryCount <= maxRetry);


            int retryCount = 0;

            do
            {
                Console.WriteLine($"Getting data (retry {retryCount}) : {url} ...");

                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Headers.Add("User-Agent", GlobalSettings.Instance.UserAgent);
                    var httpResponse = httpClient.SendAsync(request).Result;
                    logs.Add($"statusCode={httpResponse.StatusCode}");
                    logs.Add($"reasonPhrase={httpResponse.ReasonPhrase}");

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine($"statusCode={httpResponse.StatusCode}");
                        Console.WriteLine($"reasonPhrase={httpResponse.ReasonPhrase}");
                        var httpBody = httpResponse.Content.ReadAsStringAsync().Result;
                        System.IO.File.WriteAllText(System.IO.Path.Combine(output, $"{itemId}.vget-response.html"), httpBody);

                        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                        {
                            System.IO.File.WriteAllText(System.IO.Path.Combine(output, $"{itemId}-404.vget-response.html"), httpBody);
                            System.IO.File.WriteAllText(System.IO.Path.Combine(output, $"{itemId}-404.vget-summary.log"),
                                $"url={url}{System.Environment.NewLine}statusCode={httpResponse.StatusCode}{System.Environment.NewLine}reasonPhrase={httpResponse.ReasonPhrase}");
                            
                            return;
                        }
                    }
                    else 
                    {
                        var httpBody = httpResponse.Content.ReadAsStringAsync().Result;
                        System.IO.File.WriteAllText(System.IO.Path.Combine(output, $"{itemId}.vget-response.html"), httpBody);

                        var dtoString = string.Empty;
                        var pattern = @"<script.*?>(.*?)<\/script>";
                        MatchCollection matches = Regex.Matches(httpBody, pattern, RegexOptions.Singleline);
                        foreach (Match match in matches)
                        {
                            string scriptContent = match.Groups[1].Value;

                            if (scriptContent.Contains("itemDto"))
                            {
                                var startToken = "{\\\"itemDto\\\"";
                                dtoString = scriptContent.Substring(scriptContent.IndexOf(startToken));
                                var lastIndex = dtoString.LastIndexOf('}');
                                dtoString = lastIndex >= 0 ? dtoString.Substring(0, lastIndex + 1) : dtoString;
                                dtoString = dtoString.Replace("\\\"", "\"").Replace("\\\\\"", "\\\"").Replace("\\\\n", "\\n");

                                System.IO.File.WriteAllText(System.IO.Path.Combine(output, $"{itemId}.vget-response.json"), dtoString);
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(dtoString))
                        {
                            var properties = DeserializeJson<PageProperties>(dtoString);

                            item = properties.ItemDto;
                        }
                    }
                }
            } while (item == null && ++retryCount <= GlobalSettings.Instance.MaxRetry);

            if (item == null)
            {
                throw new Exception("Unexpected error.");
            }

            using (var client = new WebClient())
            {
                articlePhotos = item.Photos.Select(x => x.FullSizeUrl).ToArray();
                profilePhoto = item.User.Photo?.FullSizeUrl;

                Console.WriteLine("Article Photos:");
                var photosCounter = 1;
                var itemMetadata = GetItemMetadata(item);
                foreach (var photo in item.Photos)
                {
                    logs.Add("image=" + photo.FullSizeUrl);
                    var fileName = GetFileNameFromUrl(photo.FullSizeUrl);
                    var photoFilename = $"{itemMetadata}-{photo.Id}-{photosCounter++}-{fileName}";

                    DownloadFile(client, photo.FullSizeUrl, System.IO.Path.Combine(output, photoFilename), GlobalSettings.Instance.Delay, GlobalSettings.Instance.MaxRetry);
                }

                Console.WriteLine("Profile Photo:");
                if (!string.IsNullOrEmpty(profilePhoto))
                {
                    logs.Add("profile=" + profilePhoto);
                    var fileName = GetFileNameFromUrl(profilePhoto);
                    var profileFilename = $"{itemMetadata}-profile-{item.User.Photo.Id}-{fileName}";

                    DownloadFile(client, profilePhoto, System.IO.Path.Combine(output, profileFilename), GlobalSettings.Instance.Delay, GlobalSettings.Instance.MaxRetry);
                }
                else
                {
                    Console.WriteLine("<no profile picture>");
                }

                System.IO.File.WriteAllLines(System.IO.Path.Combine(output, $"{itemId}.vget-summary.log"), logs);
            }
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

        public static string ExtractContent(string content, string startToken, string endToken)
        {
            var startTokenLength = startToken.Length;
            var startIndex = content.IndexOf(startToken);
            if (startIndex == -1)
            {
                return null;
            }

            var result = content.Substring(startIndex + startTokenLength);
            var endIndex = result.IndexOf(endToken);

            if (endIndex == -1)
            {
                return null;
            }

            return result.Substring(0, endIndex);
        }

        internal static void GetFavorites(string operation, string itemLimit, string userId, string userCookies)
        {
            Console.WriteLine($"UserId : {userId}");
            Console.WriteLine($"Cookies : \r\n{userCookies}");

            //var url0 = $"https://www.vinted.nl/api/v2/users/{userId}/msg_threads";
            var pageCounter = 1;
            var perPages = 200;
            var itemsInLastPage = 0;
            var csvLines = new List<string>();
            var sortedLines = new Dictionary<string, List<string>>();
            var itemLimitReach = false;
            do
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.Cookie, userCookies);
                    client.Headers.Add("Accept-Encoding", $"identity");
                    client.Headers.Add("Accept-Language", GlobalSettings.Instance.AcceptLanguage);
                    client.Headers.Add("User-Agent", GlobalSettings.Instance.UserAgent);

                    var url = $"{GlobalSettings.Instance.Authority}/api/v2/users/{userId}/items/favourites?page={pageCounter}&include_sold=true&per_page={perPages}";
                    var filename = System.IO.Path.Combine(GlobalSettings.Instance.Output, $"{userId}-favorites-{pageCounter}.vget-response.json");
                    var json = DownloadString(client, filename, url, GlobalSettings.Instance.Delay, GlobalSettings.Instance.MaxRetry);

                    if (string.IsNullOrEmpty(json))
                    {
                        break;
                    }

                    var response = DeserializeJson<Domain.Model.FavouritesResponse>(json);
                    itemsInLastPage = response.Items.Count();
                    Console.WriteLine($"Page {pageCounter} : {itemsInLastPage} item(s)");
                    pageCounter++;
                    foreach (var item in response.Items)
                    {
                        var line = string.Empty;
                        var brand = CleanString(item.Brand.Title);
                        var size = CleanString(item.Size);
                        var title = item.Title.Replace(";", "");

                        if (operation == "list-csv")
                        {
                            csvLines.Add($"{item.Id};{brand};{size};{title};{item.Url}");
                        }
                        else
                        {
                            if (!sortedLines.ContainsKey(brand))
                            {
                                sortedLines.Add(brand, new List<string>());
                            }

                            sortedLines[brand].Add(item.Url);
                        }

                        if (item.Url == itemLimit)
                        {
                            itemLimitReach = true;
                            break;
                        }
                    }
                }
            }
            while (!itemLimitReach && itemsInLastPage > 0);

            if (operation == "list-csv")
            {
                var fileContent = string.Join("\r\n", csvLines);
                var filePath = System.IO.Path.Combine(GlobalSettings.Instance.Output, $"favourites-{userId}.csv");
                Console.WriteLine($"Save to {filePath}");
                Console.WriteLine($"{csvLines.Count()} item(s)");
                System.IO.File.WriteAllText(filePath, fileContent);
            }
            else
            {
                var folderMapping = new Dictionary<string, string>();
                if (System.IO.File.Exists(Path.Combine(GlobalSettings.Instance.SettingsFolder, "batchfile.foldermapping.txt")))
                {
                    var folderMappingDefinitions = System.IO.File.ReadAllLines(Path.Combine(GlobalSettings.Instance.SettingsFolder, "batchfile.foldermapping.txt"));
                    Console.WriteLine("Load folder mapping file");
                    foreach (var definition in folderMappingDefinitions)
                    {
                        var splittedDefinition = definition.Split(':');
                        if (splittedDefinition.Length == 2 && splittedDefinition[0].Length > 0)
                        {
                            Console.WriteLine(definition);
                            folderMapping.Add(splittedDefinition[0], splittedDefinition[1]);
                        }
                    }

                    Console.WriteLine("Folder mapping file loaded");
                }

                var counter = 0;
                var lines = new List<string>();
                lines.Add($"#");
                lines.Add($"# vget batch file.");
                lines.Add($"# Generated at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ");
                lines.Add($"#");
                foreach (var brand in sortedLines.Keys)
                {
                    var folderName = folderMapping.ContainsKey(brand) ? folderMapping[brand] : brand;
                    lines.Add($":{folderName}");

                    foreach (var item in sortedLines[brand])
                    {
                        lines.Add(item);
                        counter++;
                    }

                    lines.Add(String.Empty);
                }

                var fileContent = string.Join("\r\n", lines);
                var filePath = System.IO.Path.Combine(GlobalSettings.Instance.Output, $"favourites-{userId}.txt");
                Console.WriteLine($"Save to {filePath}");
                Console.WriteLine($"{counter} item(s)");
                System.IO.File.WriteAllText(filePath, fileContent);
            }
        }

        internal static void GetThreadImages(string userId, string userCookies, string threadId)
        {
            Console.WriteLine($"UserId : {userId}");
            Console.WriteLine($"UserThreadId : {threadId}");
            Console.WriteLine($"Cookies : \r\n{userCookies}");

            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Cookie, userCookies);
                client.Headers.Add("Accept-Encoding", $"identity");
                client.Headers.Add("Accept-Language", GlobalSettings.Instance.AcceptLanguage);
                client.Headers.Add("User-Agent", GlobalSettings.Instance.UserAgent);
                //var url0 = $"https://www.vinted.nl/api/v2/users/{userId}/msg_threads";
                var url = $"{GlobalSettings.Instance.Authority}/api/v2/users/{userId}/msg_threads/{threadId}";
                Console.WriteLine(url);
                var json = client.DownloadString(url);
                var response = DeserializeJson<Domain.Model.MessageThreadResponse>(json);

                //System.IO.Directory.CreateDirectory(System.IO.Path.Combine(output, threadId));
                System.IO.File.WriteAllText(System.IO.Path.Combine(GlobalSettings.Instance.Output, $"{response.MessageThread.Item.Id}-{threadId}.vget-response.json"), json);

                var itemMetadata = GetItemMetadata(response.MessageThread.Item);
                var photoCounter = 0;
                foreach (var message in response.MessageThread.Messages)
                {
                    if (message?.MessageEntity?.Photos != null && message.MessageEntity.Photos.Any())
                    {
                        foreach (var photo in message.MessageEntity.Photos)
                        {
                            photoCounter++;
                            Console.WriteLine(photo.FullSizeUrl);
                            var photoFilename = photo.FullSizeUrl.Split('/').Last();
                            photoFilename = photoFilename.Split('?').First();
                            photoFilename = $"{itemMetadata}-threadPhoto-{photoFilename.Split('.')[0]}-{photoCounter}.{photoFilename.Split('.')[1]}";

                            DownloadFile(client, photo.FullSizeUrl, System.IO.Path.Combine(GlobalSettings.Instance.Output, photoFilename), GlobalSettings.Instance.Delay, GlobalSettings.Instance.MaxRetry);
                        }
                    }
                }
            }
        }
    }

}
