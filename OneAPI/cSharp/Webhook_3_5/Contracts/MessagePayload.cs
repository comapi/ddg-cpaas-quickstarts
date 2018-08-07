using System;
using System.Runtime.Serialization;

namespace Webhook_3_5.Contracts
{
    [DataContract]
    public class MessagePayload
    {
        [DataMember]
        public Guid id { get; set; }
        [DataMember]
        public ChannelDetails details { get; set; }
        /// <summary>
        /// You will need to replace this type with a DataContract to match your metadata if you use it.
        /// </summary>
        [DataMember]
        public object metadata { get; set; }
        [DataMember]
        public string updatedOn { get; set; }
        [DataMember]
        public string conversationId { get; set; }
    }
}
