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
    public class ItemResponse
    {
        [DataMember(Name = "props")]
        public Properties Properties { get; set; }
    }
}
