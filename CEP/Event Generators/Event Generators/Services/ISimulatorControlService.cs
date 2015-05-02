using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace CEP.EventGenerators.Adaptors
{
    [ServiceContract]
    public interface ISimulatorControlService
    {
        [OperationContract]
        Boolean StartSimulator(int processCount, int taskCount, int errorPercentage, int delay);

    }
}
