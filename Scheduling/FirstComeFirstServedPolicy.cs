using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class FirstComeFirstServedPolicy : SchedulingPolicy
    {
        private Queue<int> qReadyQueue;

        public FirstComeFirstServedPolicy()
        {
            qReadyQueue = new Queue<int>();
        }

        public override void AddProcess(int iProcessId)
        {
            qReadyQueue.Enqueue(iProcessId);
        }

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            while (qReadyQueue.Count > 0)
            {
                int pid = qReadyQueue.Peek();

                if (dProcessTable.ContainsKey(pid)
                    && !dProcessTable[pid].Blocked
                    && !dProcessTable[pid].Done)
                {
                    return pid;
                }

                qReadyQueue.Dequeue();
            }

            return -1;
        }

        public override bool RescheduleAfterInterrupt()
        {
            return false;
        }
    }
}