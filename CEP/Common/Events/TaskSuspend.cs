using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class TaskSuspend : TaskEvent
    {
        public TaskSuspend(object processID, object taskID, object taskType, object taskActor)
            : base(processID, taskID, taskType, taskActor)
        {
        }
    }
}
