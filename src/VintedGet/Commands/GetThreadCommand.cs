using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Infrastructure;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class GetThreadCommand
    {
        public void Execute(string threadId)
        {
            var session = VintedProcessor.GetSession();
            if (session != null)
            {
                VintedProcessor.GetThreadImages(session.UserId, session.Cookies, threadId);
            }
        }
    }
}
