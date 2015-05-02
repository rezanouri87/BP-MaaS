using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CEP.Server.Adaptor
{
    [ServiceContract]
    public interface IStatementManagerService
    {
        [OperationContract]
        Boolean CreateStatement(String name, String statement);

        [OperationContract]
        Boolean CreateXMLStatement(String name, XmlNode LHSRuleNode, XmlNode RHSRuleNode, Dictionary<string, string> prams);

        [OperationContract]
        void DeleteStatements();

        [OperationContract]
        IDictionary<String, String> GetStatements();

        [OperationContract]
        void StopStatement(String name);

        [OperationContract]
        void StartStatement(String name);

        [OperationContract]
        void StopAllStatements();

        [OperationContract]
        void StartAllStatements();
    }

}
