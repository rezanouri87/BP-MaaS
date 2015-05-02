using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class TaskComplete : TaskEvent
    {
        public TaskComplete(object processID, object taskID, object taskType, object taskActor)
            : base(processID, taskID, taskType, taskActor)
        {
        }
        public TaskComplete(object processID, object taskID, object taskType, object taskActor, object Resource)
            : base(processID, taskID, taskType, taskActor, Resource)
        {
        }
    }
}
