using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CEP.Server.Adaptor;
using CEP.Server.Services;
using com.espertech.esper.client;
using log4net;

namespace CEP.Server
{
    class Adaptors
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Start()
        {
            // get the <system.serviceModel> / <services> config section
            var services = ConfigurationManager.GetSection("system.serviceModel/services") as ServicesSection;

            // get all classs
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(t => t.IsClass == true).ToList();

            // enumerate over each <service> node
            foreach (ServiceElement service in services.Services)
            {
                var serviceType = allTypes.SingleOrDefault(t => t.FullName == service.Name);
                if (serviceType == null)
                {
                    continue;
                }

                var serviceHost = new ServiceHost(serviceType);
                try
                {
                    serviceHost.Open();
                    Log.InfoFormat("Open Service Host for {0} on (at least) {1}", serviceType.Name,
                        serviceHost.BaseAddresses.FirstOrDefault());
                }
                catch (Exception e)
                {
                    Log.InfoFormat("Exception {0} with Message: {1} when starting WFC service", e.ToString(),
                        e.Message);
                }

            }
        }
    }
}
