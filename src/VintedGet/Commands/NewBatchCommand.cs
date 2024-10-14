using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class NewBatchCommand
    {
        public void Execute(string filename)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(GlobalSettings.Instance.SettingsFolder, "BatchTemplate.txt")))
            {
                System.IO.File.Copy(System.IO.Path.Combine(GlobalSettings.Instance.SettingsFolder, "BatchTemplate.txt"), filename, true);
            }
            else
            {
                System.IO.File.WriteAllText(filename, Infrastructure.TextResource.ReadContent(Assembly.GetExecutingAssembly(), "VintedGet.BatchTemplate.txt"));
            }

            Console.WriteLine($"Batch template created : {filename}");
        }
    }
}
