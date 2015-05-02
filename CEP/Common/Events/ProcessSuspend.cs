using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class ProcessSuspend : ProcessEvent
    {
        public ProcessSuspend(object identifier)
            : base(identifier)
        {
           
        }
    }
}
