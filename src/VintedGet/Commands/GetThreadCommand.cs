using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class GetThreadCommand
    {
        public void Execute(string userId, string userSession, string threadId)
        {

            VintedProcessor.EnsureHasSession(ref userId, ref userSession);
            if (VintedProcessor.HasSession(userId, userSession))
            {
                VintedProcessor.GetThreadImages(userId, userSession, threadId);
            }
        }
    }
}
