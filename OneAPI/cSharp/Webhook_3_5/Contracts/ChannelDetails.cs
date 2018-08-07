using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Webhook_3_5.Contracts
{
    [DataContract]
    public class ChannelDetails
    {
        [DataMember]
        public string channel { get; set; }
        [DataMember]
        public object additionalInfo { get; set; }
        [DataMember]
        public object channelStatus { get; set; }
    }
}
