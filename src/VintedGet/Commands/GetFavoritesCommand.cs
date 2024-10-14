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
        public void Execute(string userId, string userSession, string operation, string itemLimit)
        {
            VintedProcessor.EnsureHasSession(ref userId, ref userSession);
            if (VintedProcessor.HasSession(userId, userSession))
            {
                VintedProcessor.GetFavorites(operation, itemLimit, userId, userSession);
            }
        }
    }
}
