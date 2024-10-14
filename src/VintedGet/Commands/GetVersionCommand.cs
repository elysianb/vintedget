using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class GetVersionCommand
    {
        public void Execute()
        {
            Console.WriteLine(GlobalSettings.Instance.GetVersion());
        }
    }
}
