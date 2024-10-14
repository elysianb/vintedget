using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class GetItemCommand
    {
        public void Execute(string url)
        {
            using (var client = new HttpClient())
            {
                VintedProcessor.GetPhotos(client, null, url);
            }
        }
    }
}
