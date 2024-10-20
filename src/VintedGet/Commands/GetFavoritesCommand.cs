using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class GetFavoritesCommand
    {
        public void Execute(string userId, string userCookies, string operation, string itemLimit)
        {
            VintedProcessor.EnsureHasSession(ref userId, ref userCookies);
            if (VintedProcessor.HasSession(userId, userCookies))
            {
                VintedProcessor.GetFavorites(operation, itemLimit, userId, userCookies);
            }
        }
    }
}
