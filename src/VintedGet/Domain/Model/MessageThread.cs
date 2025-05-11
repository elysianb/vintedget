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
    public class MessageThread
    {
        [DataMember(Name = "transaction")]
        public TransactionDto Transaction { get; set; }

        [DataMember(Name = "messages")]
        public MessageBody[] Messages { get; set; }
    }

    [DataContract]
    public class TransactionDto
    {
        [DataMember(Name = "item_id")]
        public string ItemId { get; set; }
    }
}
