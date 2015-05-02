using System.Runtime.Serialization;

namespace CEP.Common.Events
{
  
    [KnownType(typeof(ProcessEvent))]
    [KnownType(typeof(ProcessStart))]
    [KnownType(typeof(ProcessSuspend))]
    [KnownType(typeof(ProcessResume))]
    [KnownType(typeof(ProcessStop))]
    [KnownType(typeof(ProcessComplete))]
    [KnownType(typeof(TaskEvent))]
    [KnownType(typeof(TaskStart))]
    [KnownType(typeof(TaskSuspend))]
    [KnownType(typeof(TaskResume))]
    [KnownType(typeof(TaskStop))]
    [KnownType(typeof(TaskComplete))]
    [KnownType(typeof(TaskResourceAllocation))]
    [DataContract]
    public abstract class RawEvent : IEsperEvent
    {
        protected RawEvent()
        {
            var typeName = this.GetType().ToString();
            this.Name = typeName.Substring(typeName.LastIndexOf('.')+1);
        }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string TimeStamp { get; set; }
        // SOLVED time stamp is already handeled in EventReceiverService
    }
}
