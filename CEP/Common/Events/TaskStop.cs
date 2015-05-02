using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class TaskStop : TaskEvent
    {
        public TaskStop(object processID, object taskID, object taskType, object taskActor)
            : base(processID, taskID, taskType, taskActor)
        {
        }
    }
}
