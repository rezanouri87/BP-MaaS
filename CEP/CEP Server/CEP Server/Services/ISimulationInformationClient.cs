using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using CEP.Common.Events;
using CEP.Common.Utils;

namespace CEP.Server.Adaptor
{
    [ServiceContract]
    public interface ISimulationInformationClient
    {
        [OperationContract(IsOneWay = true)]
        void PingDashboardVoid();

        [OperationContract(IsOneWay = true)]
        void ReceiveEventData(RawEvent e);

        [OperationContract(IsOneWay = true)]
        void ReceiveNotificationDictionary(string statementName, Dictionary<String, object> dict);

        [OperationContract(IsOneWay = true)]
        void ReceiveEventChange(Dictionary<String, object> dict);
    }
}
