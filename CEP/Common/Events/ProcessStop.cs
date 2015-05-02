using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class ProcessStop : ProcessEvent
    {
        public ProcessStop(object identifier)
            : base(identifier)
        {

        }
    }
}
