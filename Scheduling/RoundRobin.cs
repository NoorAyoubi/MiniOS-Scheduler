using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    internal class RoundRobinPolicy : SchedulingPolicy
    {
        private Queue<int> qReadyQueue;
        private int iQuantum;

        public RoundRobinPolicy(int quantum)
        {
            iQuantum = quantum;
            qReadyQueue = new Queue<int>();
        }

        public int Quantum
        {
            get { return iQuantum; }
        }

        public override void AddProcess(int iProcessId)
        {
            qReadyQueue.Enqueue(iProcessId);
        }

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            // Clean the queue - remove processes that are no longer ready
            int iOriginalCount = qReadyQueue.Count;

            for (int i = 0; i < iOriginalCount; i++)
            {
                int iNextPid = qReadyQueue.Dequeue();

                if (dProcessTable.ContainsKey(iNextPid) &&
                    !dProcessTable[iNextPid].Blocked &&
                    !dProcessTable[iNextPid].Done)
                {
                    // Round Robin: put the process back at the end of the queue
                    qReadyQueue.Enqueue(iNextPid);

                    // Important: Do NOT set Quantum here!
                    // The CPU and OS are responsible for:
                    // 1. Setting CPU.RemainingTime = this.Quantum when process starts
                    // 2. Decrementing CPU.RemainingTime each tick
                    // 3. Calling TimeoutReached() when CPU.RemainingTime == 0

                    return iNextPid;
                }
            }

            return -1;
        }

        public override bool RescheduleAfterInterrupt()
        {
            // Round Robin - Preemptive scheduling
            // After any interrupt (including timer/quantum expiration), reschedule
            return true;
        }
    }
}
