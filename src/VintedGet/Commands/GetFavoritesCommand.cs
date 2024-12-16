using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Infrastructure;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class GetFavoritesCommand
    {
        public void Execute(string operation, string itemLimit)
        {
            var session = VintedProcessor.GetSession();
            if (session != null)
            {
                VintedProcessor.GetFavorites(operation, itemLimit, session.UserId, session.Cookies);
            }
        }
    }
}
