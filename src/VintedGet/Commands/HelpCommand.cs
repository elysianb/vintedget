using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class HelpCommand
    {
        public void Execute()
        {
            Console.WriteLine(Infrastructure.TextResource.ReadContent(Assembly.GetExecutingAssembly(), "VintedGet.Manual.txt"));
        }
    }
}
