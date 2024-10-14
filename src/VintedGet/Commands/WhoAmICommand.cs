using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class WhoAmICommand
    {
        public void Execute()
        {
            string userId = null;
            string userSession = null;
            VintedProcessor.EnsureHasSession(ref userId, ref userSession);
            if (VintedProcessor.HasSession(userId, userSession))
            {
                var user = VintedProcessor.GetUser(userId, userSession);
                Console.WriteLine($"logged as {user.Login}");
            }
            else
            {
                Console.WriteLine($"not connected");
            }
        }
    }
}
