using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class LogoutCommand
    {
        public void Execute()
        {
            VintedProcessor.Logout();
        }
    }
}
