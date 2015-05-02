
using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class ProcessEvent : RawEvent
    {
        [DataMember]
        public object ProcessID { get; set; }

        public ProcessEvent(object processID)
            : base()
        {
            this.ProcessID = processID;

        }
    }
}
