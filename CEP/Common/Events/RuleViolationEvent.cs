using System.Runtime.Serialization;



namespace CEP.Common.Events
{
    // [DONE] ToDo: [Ahmed Awad]: I have created this new event type to define what the stream RuleViolationEvent is. This stream will be fed by events when a violation occurs.
    class RuleViolationEvent :IEsperEvent
    {
        [DataMember]
        public string processID { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string RuleID { get; set; }

        [DataMember]
        public string RuleType { get; set; }

    }
}
