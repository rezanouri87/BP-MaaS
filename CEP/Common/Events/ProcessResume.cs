using System.Runtime.Serialization;

namespace CEP.Common.Events
{
    [DataContract]
    public class ProcessResume : ProcessEvent
    {
        public ProcessResume(object identifier)
            : base(identifier)
        {

        }
    }
}
