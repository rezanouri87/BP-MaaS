using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class TaskEvent : RawEvent
    {
        [DataMember]
        public object ProcessID { get; set; }

        [DataMember]
        public object TaskID { get; set; }

        [DataMember]
        public object Task { get; set; }

        [DataMember]
        public object TaskActor { get; set; }

        [DataMember]
        public object Resource { get; set; }

        [DataMember]
        public object Role { get; set; }
        


        public TaskEvent(object processID, object taskID, object taskType, object taskActor)
            : base()
        {
            this.ProcessID = processID;
            this.TaskID = taskID;
            this.Task = taskType;
            this.Resource = taskActor;
            this.Role = "Default Role";
        }

        public TaskEvent(object processID, object taskID, object taskType, object taskActor, object Role)
            : base()
        {
            this.ProcessID = processID;
            this.TaskID = taskID;
            this.Task = taskType;
            this.Resource = taskActor;
            this.Role = Role;
        }
    }
}
