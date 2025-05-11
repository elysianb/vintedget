using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VintedGet.Domain.Model
{
    [DataContract]
    public class PageProperties
    {
        [DataMember(Name = "itemDto")]
        public ItemDto ItemDto { get; set; }

        [DataMember(Name = "plugins")]
        public PluginDto[] Plugins { get; set; }
    }
}
