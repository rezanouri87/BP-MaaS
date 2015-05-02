using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class TaskResourceAllocation : TaskEvent
    {
        public TaskResourceAllocation (object processID, object taskID, object taskType, object taskActor)
            : base(processID, taskID, taskType, taskActor)
        {
        }
    }
}
