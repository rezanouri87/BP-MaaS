using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using CEP.EventGenerators.Adaptors;
using CEP.EventGenerators.Simulations;

namespace CEP.EventGenerators.Services
{
    class SimulatorControlService : ISimulatorControlService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ProcessSimulator _processSimulator;

        public SimulatorControlService() 
        {
           this._processSimulator = new ProcessSimulator();
        }

        public bool StartSimulator(int processCount, int taskCount, int errorPercentage, int delay)
        {
            Log.Info("Starting Simulator...");
            if (!this._processSimulator.Started){ 
                this._processSimulator.StartSimulator( processCount, taskCount, errorPercentage, delay);
            }
            else 
            {
                this._processSimulator.UpdateParameters(processCount, taskCount, errorPercentage, delay);
            }
            return true;
        }

       
    }
}
