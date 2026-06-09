using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class PrioritizedScheduling : SchedulingPolicy
    {
        private int iQuantum;
        private Dictionary<int, Queue<int>> dPriorityQueues;
        private SortedSet<int> sPriorities;

        public PrioritizedScheduling(int iQuantum)
        {
            this.iQuantum = iQuantum;
            dPriorityQueues = new Dictionary<int, Queue<int>>();
            sPriorities = new SortedSet<int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
        }

        public override void AddProcess(int iProcessId)
        {
            AddProcessWithPriority(iProcessId, 0);
        }

        public void AddProcessWithPriority(int iProcessId, int iPriority)
        {
            if (!dPriorityQueues.ContainsKey(iPriority))
            {
                dPriorityQueues[iPriority] = new Queue<int>();
                sPriorities.Add(iPriority);
            }

            dPriorityQueues[iPriority].Enqueue(iProcessId);
        }

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            foreach (int iPriority in sPriorities)
            {
                if (dPriorityQueues.ContainsKey(iPriority) && dPriorityQueues[iPriority].Count > 0)
                {
                    Queue<int> qQueue = dPriorityQueues[iPriority];
                    int iOriginalCount = qQueue.Count;

                    for (int i = 0; i < iOriginalCount; i++)
                    {
                        int iNextPid = qQueue.Dequeue();

                        if (dProcessTable.ContainsKey(iNextPid) &&
                            !dProcessTable[iNextPid].Blocked &&
                            !dProcessTable[iNextPid].Done)
                        {
                            qQueue.Enqueue(iNextPid);
                            return iNextPid;
                        }
                    }
                }
            }

            return -1;
        }

        public override bool RescheduleAfterInterrupt()
        {
            return true;
        }
    }
}