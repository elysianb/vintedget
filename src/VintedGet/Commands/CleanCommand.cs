using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VintedGet.Commands
{
    internal class CleanCommand
    {
        public void Execute(string directory)
        {
            var patterns = new[] { 
                ".vget-summary.log", 
                ".vget-response.json", 
                ".vget-response.html" 
            };

            var deletedCounter = 0;

            foreach (var pattern in patterns)
            {
                var summaryFiles = Directory.EnumerateFiles(directory, "*" + pattern, SearchOption.AllDirectories);
                foreach (var file in summaryFiles)
                {
                    if (!file.EndsWith(pattern))
                    {
                        continue;
                    }

                    Console.WriteLine($"Removing {file}");
                    System.IO.File.Delete(file);
                    deletedCounter++;
                }
            }

            Console.WriteLine($"{deletedCounter} file(s) deleted");
        }
    }
}
