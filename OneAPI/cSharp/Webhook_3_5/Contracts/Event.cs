using System;
using System.Runtime.Serialization;

namespace Webhook_3_5.Contracts
{
    /// <summary>
    /// This is a serializable class to represent a Comapi webhook events payload
    /// </summary>
    [DataContract]
    public class Event
    {
        [DataMember]
        public Guid eventId { get; set; }
        [DataMember]
        public int accountId { get; set; }
        [DataMember]
        public Guid apiSpaceId { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public MessagePayload payload { get; set; }
        [DataMember]
        public int revision { get; set; }
        [DataMember]
        public string etag { get; set; }
        [DataMember]
        public string timestamp { get; set; }
    }
}
