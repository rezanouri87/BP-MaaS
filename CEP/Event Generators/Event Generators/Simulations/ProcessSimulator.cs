 using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Timers;
using CEP.Common.Events;
using CEP.Common.Utils;
using CEP.EventGenerators.EventReceiverService;
using log4net.Core;
using Timer = System.Threading.Timer;
using CEP.Common;
using Process = CEP.Common.Process;


namespace CEP.EventGenerators.Simulations
{
    internal class ProcessSimulator
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string[] TaskTypes = { "A", "B", "C", "D"};
        private static readonly string[] TaskActors = { "A1", "A2", "A3", "A4"};

        private int _taskCount;
        private int _processCount;
        private int _delay;
        private int _errorPercentage;

        public bool Started { get; set; }

        private List<Task> _tasks;
        private List<Process> _processes;

        private Adaptor adaptor = new Adaptor();

        public ProcessSimulator()
        {
            _processes = new List<Process>();
            _tasks = new List<Task>();
            Log.DebugFormat("Initializing Event Generator");
        }

        public void StartSimulator(int processCount, int taskCount, int errorPercentage, int delay)
        {
            this._taskCount = taskCount;
            this._processCount = processCount;
            this._delay = delay;
            this._errorPercentage = errorPercentage;

            Log.DebugFormat("Starting Update-Loop-Thread: {0}", this.ToString());
            this.Started = true;

            var threadStart = new ThreadStart(LoopUpdate);
            var thread = new Thread(threadStart);
            thread.Start();
        }

        public void UpdateParameters(int processCount, int taskCount, int errorPercentage, int delay)
        {
            this._taskCount = taskCount;
            this._processCount = processCount;
            this._delay = delay;
            this._errorPercentage = errorPercentage;

            //
            this.Started = true;

            var threadStart = new ThreadStart(LoopUpdate);
            var thread = new Thread(threadStart);
            thread.Start();
        }

        private void LoopUpdate()
        {
            //while (true)
            //{
            //    Thread.Sleep(this._delay);
            //    this.Update();
            //}


            // *********************************** violation sequance *******************************
            adaptor.SendEvent(new ProcessStart("1")); Thread.Sleep(this._delay);
            adaptor.SendEvent(new TaskComplete("1", "1", "C", "Ahmed")); Thread.Sleep(this._delay);
            adaptor.SendEvent(new TaskComplete("1", "2", "A", "Samir","Engineer")); Thread.Sleep(3000);
            adaptor.SendEvent(new TaskComplete("1", "3", "B", "Ahmed")); Thread.Sleep(this._delay);
            //adaptor.SendEvent(new TaskComplete("1", "4", "A", "Ahmed")); Thread.Sleep(this._delay);
            adaptor.SendEvent(new TaskComplete("1", "5", "A", "Ahmed")); Thread.Sleep(this._delay);
            adaptor.SendEvent(new TaskComplete("1", "6", "M", "Ahmed")); Thread.Sleep(this._delay);
            //adaptor.SendEvent(new TaskComplete("1", "7", "A", "Ahmed")); Thread.Sleep(this._delay);
            adaptor.SendEvent(new ProcessComplete("1")); Thread.Sleep(this._delay);



            // *********************************** NON violation sequance *******************************
            //adaptor.SendEvent(new ProcessStart("1")); Thread.Sleep(this._delay);
            //adaptor.SendEvent(new TaskComplete("1", "1", "C", "Ahmed")); Thread.Sleep(this._delay);
            //adaptor.SendEvent(new TaskComplete("1", "2", "A", "Ahmed")); Thread.Sleep(this._delay);
            //adaptor.SendEvent(new TaskComplete("1", "4", "B", "Ahmed")); Thread.Sleep(this._delay);
            //adaptor.SendEvent(new TaskComplete("1", "3", "A", "Ahmed")); Thread.Sleep(this._delay);
            //adaptor.SendEvent(new TaskComplete("1", "5", "D", "Ahmed")); Thread.Sleep(this._delay);
            //adaptor.SendEvent(new TaskComplete("1", "6", "E", "Ahmed")); Thread.Sleep(this._delay);
            //adaptor.SendEvent(new ProcessComplete("1")); Thread.Sleep(this._delay);


        }

        // this is very complex and ugly for simulation purpose
        protected void Update()
        {
            // clean
            _tasks.RemoveAll(t => t.Index == -1);
            _tasks.RemoveAll(t => _processes.Any(p => t.ProcessID == p.ID && p.Index == -1));
            _processes.RemoveAll(p => p.Index == -1);

            var rand100 = UniformRandom.Rand.Next(0, 100);
            if (rand100 < 30)
            {
                // process
                if (rand100 < 10)
                {
                    // add
                    if (_processes.Count < _processCount)
                    {
                        var p = new Process();
                        _processes.Add(p);
                        adaptor.SendEvent(getProcessEvent(p));
                    }
                }
                else
                {
                    if (_processes.Count > 0)
                    {
                        // update 
                        var i = UniformRandom.Rand.Next(0, _processes.Count);
                        if (_processes[i].Index == -1)
                        {
                            // Remove completed processes
                            _processes.RemoveAt(i);
                        }
                        var PE = getProcessEvent(_processes[i]);
                        if (PE is ProcessComplete)
                        {
                            foreach (var t in _tasks.Where(x => x.ProcessID == PE.ProcessID))
                            {
                                adaptor.SendEvent(new TaskComplete(t.ProcessID, t.ID, t.TaskType, t.TaskActor));
                            }
                            _tasks.RemoveAll(ts => ts.ProcessID == PE.ProcessID);
                        }                        
                        adaptor.SendEvent(PE);

                    }
                }
            }
            else
            {
                //Task
                if (_processes.Count <= 0) return;

                // parent process
                var ix = UniformRandom.Rand.Next(0, _processes.Count - 1);
                if (_processes[ix].Index == -1)
                {
                    // Remove completed processes
                    _tasks.RemoveAll(ts => ts.ProcessID == _processes[ix].ID);
                    _processes.RemoveAt(ix);
                    return;
                }
                var p = _processes[ix];

                var tasksOfP = _tasks.Where(z => z.ProcessID == p.ID).ToList();
                var count = tasksOfP.Count();

                if (rand100 > 90)
                {
                    // add
                    if (count < _taskCount)
                    {
                        var t = new Task();
                        t.ProcessID = p.ID;
                        t.TaskType = TaskTypes[UniformRandom.Rand.Next(0, 4)];
                        t.TaskActor = TaskActors[UniformRandom.Rand.Next(0, 4)];
                        _tasks.Add(t);
                        adaptor.SendEvent(getTaskEvent(t));
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        // update 
                        var i = UniformRandom.Rand.Next(0, count);
                        if (tasksOfP[i].Index == -1)
                        {
                            // Remove completed taskes
                            _tasks.Remove(tasksOfP[i]);
                            return;
                        }
                        adaptor.SendEvent(getTaskEvent(tasksOfP[i]));
                    }
                }
            }
        }

        private TaskEvent getTaskEvent(Task t)
        {
            t.Index = GenerateRandomErrorIndex(0, 5, t.Index);
            switch (t.Index)
            {
                case 0:
                    t.Index = 1; // can jump to 
                    return new TaskResourceAllocation(t.ProcessID, t.ID, t.TaskType, t.TaskActor);
                case 1:
                    t.Index = selectRand(2, 4, 5); // can jump to 
                    return new TaskStart(t.ProcessID, t.ID, t.TaskType, t.TaskActor);
                case 2:
                    t.Index = 3; // can jump to 
                    return new TaskSuspend(t.ProcessID, t.ID, t.TaskType, t.TaskActor);
                case 3:
                    t.Index = selectRand(2, 4, 5); // can jump to 
                    return new TaskResume(t.ProcessID, t.ID, t.TaskType, t.TaskActor);
                case 4:
                    t.Index = 1; // can jump to 
                    return new TaskStop(t.ProcessID, t.ID, t.TaskType, t.TaskActor);
                default:
                    t.Index = -1; // mark for remove 
                    return new TaskComplete(t.ProcessID, t.ID, t.TaskType, t.TaskActor);
            }
        }

        private ProcessEvent getProcessEvent(Process p)
        {
            p.Index = GenerateRandomErrorIndex(1, 5, p.Index);
            switch (p.Index)
            {
                case 1:
                    p.Index = selectRand(2, 4, 5); // can jump to 
                    return new ProcessStart(p.ID);
                case 2:
                    p.Index = 3; // can jump to 
                    return new ProcessSuspend(p.ID);
                case 3:
                    p.Index = selectRand(2, 4, 5); // can jump to 
                    return new ProcessResume(p.ID);
                case 4:
                    p.Index = 1;// can jump to 
                    return new ProcessStop(p.ID);
                default:
                    p.Index = -1; // mark for remove 
                    return new ProcessComplete(p.ID);
            }
        }

        private int GenerateRandomErrorIndex(int from, int to, int index)
        {
            // error factor to generate random
            if (UniformRandom.Rand.Next(0, 100) < this._errorPercentage % 100)
            {
                return (UniformRandom.Rand.Next(from, to));
            }
            else
            {
                return index;
            }
        }

        private int selectRand(params int[] list)
        {
            return list[UniformRandom.Rand.Next(0, list.Length)];
        }


    }
}

