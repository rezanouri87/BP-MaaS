using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class ProcessComplete : ProcessEvent
    {
        public ProcessComplete(object identifier)
            : base(identifier)
        {

        }
    }
}
