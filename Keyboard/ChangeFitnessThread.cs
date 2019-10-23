using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keyboard
{
    class ChangeFitnessThread
    {

        volatile bool _shouldStop = false;
        int timeIntervalMs;
        Form1 caller;

        public ChangeFitnessThread(int timeS, Form1 caller) // time is passed in seconds
        {
            timeIntervalMs = timeS * 1000;
            this.caller = caller;
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }

        public void doWork() 
        {
            while (!_shouldStop)
            {
                Thread.Sleep(timeIntervalMs);
                if (!_shouldStop)
                    caller.setNewFitness();
            }
        }
    }
}
