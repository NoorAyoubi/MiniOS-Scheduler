using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    // המחלקה האבסטרקטית כפי שהוגדרה בתרגיל
    abstract class SchedulingPolicy  // 
    {
        public abstract int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable);
        public abstract void AddProcess(int iProcessId);
        public abstract bool RescheduleAfterInterrupt();
    }
}