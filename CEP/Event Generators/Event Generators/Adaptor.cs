using CEP.Common.Events;
using CEP.EventGenerators.Adaptors;
using CEP.EventGenerators.EventReceiverService;
using CEP.EventGenerators.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RawEvent = CEP.Common.Events.RawEvent;

namespace CEP.EventGenerators
{
    public class Adaptor
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Host the generatore services 
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

                Log.InfoFormat("Open Service Host for {0} on (at least) {1}", serviceType.Name,
                    serviceHost.BaseAddresses.FirstOrDefault());
                serviceHost.Open();
            }
        }

        EventReceiverServiceClient service = new EventReceiverService.EventReceiverServiceClient();

        int eventsSentCount = 0;

        object reconnectLock = new object();

        public void SendEvent(RawEvent rEvent)
        {
            try
            {
                service.SendEvent(rEvent);

                Log.DebugFormat("Send Event #{1}: {0}", rEvent.ToString(), ++eventsSentCount);
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Sending Event failed: {0}", e.Message);

                lock (reconnectLock)
                {
                    if (service.State == System.ServiceModel.CommunicationState.Faulted)
                    {
                        Log.Warn("Creating new service client");
                        service = new EventReceiverService.EventReceiverServiceClient();
                    }
                }
            }
        }
    }
}
