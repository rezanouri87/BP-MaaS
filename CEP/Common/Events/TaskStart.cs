using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class TaskStart : TaskEvent
    {
        public TaskStart(object processID, object taskID, object taskType, object taskActor)
            : base(processID, taskID, taskType, taskActor)
        {
        }
    }
}
