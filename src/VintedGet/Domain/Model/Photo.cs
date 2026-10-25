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
        private string _fullSizeUrl = null;

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "full_size_url")]
        public string FullSizeUrl 
        {
            get { return _fullSizeUrl?? Url; }
            set { _fullSizeUrl = value; } 
        }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
