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
    public class Photo
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "full_size_url")]
        public string FullSizeUrl { get; set; }
    }
}
