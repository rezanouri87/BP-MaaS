using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CEP.EventGenerators.Services;
using log4net;

namespace CEP.EventGenerators
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            Log.Info("Event Generator starting up...");

            Log.Info("Starting Adaptor...");
            var tcpAdaptor = new Adaptor();
            tcpAdaptor.Start();

            SimulatorControlService service = new SimulatorControlService();
            service.StartSimulator(3, 3, 10, 500);
            Log.Info("Event Generator ready!");
            Console.ReadLine();
        }
    }
}
