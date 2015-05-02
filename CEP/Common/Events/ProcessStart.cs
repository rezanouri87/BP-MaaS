using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class ProcessStart : ProcessEvent
    {
        public ProcessStart(object identifier)
            : base(identifier)
        {

        }
    }
}
