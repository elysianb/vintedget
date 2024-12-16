using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Infrastructure;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class WhoAmICommand
    {
        public void Execute()
        {
            var session = VintedProcessor.GetSession();
            if (session != null)
            {
                var user = VintedProcessor.GetUser(session.UserId, session.Cookies);
                Console.WriteLine($"logged as {user.Login}");
            }
            else
            {
                Console.WriteLine($"not connected");
            }
        }
    }
}
