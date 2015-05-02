using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class TaskResume : TaskEvent
    {
        public TaskResume(object processID, object taskID, object taskType, object taskActor)
            : base(processID, taskID, taskType, taskActor)
        {
        }
    }
}
