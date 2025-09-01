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

        [DataMember(Name = "brand_title")]
        public string BrandTitle { get; set; }

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

        [DataMember(Name = "seller_photo")]
        public Photo SellerPhoto { get; set; }
    }

    [DataContract]
    public class PluginDto
    {
        [DataMember(Name = "data")]
        public DataDto Data { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }

    [DataContract]
    public class DataDto
    {
        [DataMember(Name = "attributes")]
        public AttributeDto[] Attributes { get; set; }
    }

    [DataContract]
    public class AttributeDto
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "data")]
        public AttributeDataDto Data { get; set; }
    }

    [DataContract]
    public class AttributeDataDto
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}

