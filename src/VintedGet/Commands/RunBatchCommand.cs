using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class RunBatchCommand
    {
        public void Execute(string filename, bool statisticsOnly)
        {
            Console.WriteLine($"Read configuration file: {filename}");
            var config = System.IO.File.ReadAllLines(filename);
            var folders = new Dictionary<string, List<string>>();
            var currentTargetFolder = ".";
            var totalItems = 0;
            folders.Add(currentTargetFolder, new List<string>());
            foreach (var line in config)
            {
                var formatedLine = line.Trim(new char[] { ' ', '\t' });
                if (string.IsNullOrEmpty(formatedLine) || formatedLine.StartsWith("#"))
                {
                    continue;
                }
                else if (formatedLine.StartsWith(":"))
                {
                    currentTargetFolder = formatedLine.Substring(1);
                    if (!folders.ContainsKey(currentTargetFolder))
                    {
                        folders.Add(currentTargetFolder, new List<string>());
                    }
                }
                else
                {
                    folders[currentTargetFolder].Add(formatedLine.Trim(' '));
                    totalItems++;
                }
            }

            foreach (var folder in folders)
            {
                Console.WriteLine($"  - {folder.Key} : {folder.Value.Count()} item(s)");
            }

            Console.WriteLine($"  Total item(s) : {totalItems}");

            if (statisticsOnly)
            {
                return;
            }

            var counter = 0;
            Console.WriteLine("Processing");
            using (var client = VintedProcessor.AquirePublicSession(out var csrfToken))
            {
                foreach (var folder in folders)
                {
                    System.IO.Directory.CreateDirectory(folder.Key);
                    var folderCounter = 0;
                    foreach (var url in folder.Value)
                    {
                        Console.WriteLine($"Item {++counter} of {totalItems} ({folder.Key} : {++folderCounter} of {folder.Value.Count()})");
                        VintedProcessor.GetPhotos(client, null, url, folder.Key);
                    }
                }
            }
        }
    }
}
