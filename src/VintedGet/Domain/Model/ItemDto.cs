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
    public class ItemDto
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "brand_dto")]
        public BrandDto Brand { get; set; }

        [DataMember(Name = "size_title")]
        public string Size { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "photos")]
        public Photo[] Photos { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }
    }
}
