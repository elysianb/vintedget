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
        public void Execute(string userId, string userCookies, string threadId)
        {

            VintedProcessor.EnsureHasSession(ref userId, ref userCookies);
            if (VintedProcessor.HasSession(userId, userCookies))
            {
                VintedProcessor.GetThreadImages(userId, userCookies, threadId);
            }
        }
    }
}
