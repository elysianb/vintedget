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
    public class Pagination
    {
        [DataMember(Name = "current_page")]
        public int CurrentPage { get; set; }

        [DataMember(Name = "per_page")]
        public int PerPage { get; set; }

        [DataMember(Name = "total_entries")]
        public int TotalEntries { get; set; }

        [DataMember(Name = "total_pages")]
        public int TotalPages { get; set; }
    }
}
