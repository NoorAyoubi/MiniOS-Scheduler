using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class OperatingSystem
    {
        public Disk Disk { get; private set; }
        public CPU CPU { get; private set; }
        private Dictionary<int, ProcessTableEntry> m_dProcessTable;
        private Queue<ReadTokenRequest> m_lReadRequests;
        private int m_cProcesses;
        private SchedulingPolicy m_spPolicy;
        private int m_iIdleProcessId;
        private bool m_bIdleProcessCreated;

        public OperatingSystem(CPU cpu, Disk disk, SchedulingPolicy sp)
        {
            CPU = cpu;
            Disk = disk;
            m_dProcessTable = new Dictionary<int, ProcessTableEntry>();
            m_lReadRequests = new Queue<ReadTokenRequest>();
            cpu.OperatingSystem = this;
            disk.OperatingSystem = this;
            m_spPolicy = sp;
            m_bIdleProcessCreated = false;

            CreateIdleProcess();
        }

        private void CreateIdleProcess()
        {
            IdleCode idleCode = new IdleCode();
            int idlePid = m_cProcesses++;
            ProcessTableEntry idleProcess = new ProcessTableEntry(idlePid, "idle", idleCode);
            idleProcess.StartTime = CPU.TickCount;
            idleProcess.ProgramCounter = 0;
            idleProcess.LastCPUTime = CPU.TickCount;
            m_dProcessTable[idlePid] = idleProcess;
            m_iIdleProcessId = idlePid;
            m_bIdleProcessCreated = true;
        }

        public void CreateProcess(string sCodeFileName)
        {
            Code code = new Code(sCodeFileName);
            int pid = m_cProcesses++;
            m_dProcessTable[pid] = new ProcessTableEntry(pid, sCodeFileName, code);
            m_dProcessTable[pid].StartTime = CPU.TickCount;
            m_dProcessTable[pid].LastCPUTime = CPU.TickCount;
            m_spPolicy.AddProcess(pid);
        }

        public void CreateProcess(string sCodeFileName, int iPriority)
        {
            Code code = new Code(sCodeFileName);
            int pid = m_cProcesses++;
            m_dProcessTable[pid] = new ProcessTableEntry(pid, sCodeFileName, code);
            m_dProcessTable[pid].Priority = iPriority;
            m_dProcessTable[pid].StartTime = CPU.TickCount;
            m_dProcessTable[pid].LastCPUTime = CPU.TickCount;
            m_spPolicy.AddProcess(pid);
        }

        public void ProcessTerminated(Exception e)
        {
            if (e != null)
                Console.WriteLine("Process " + CPU.ActiveProcess + " terminated unexpectedly. " + e);

            if (m_dProcessTable.ContainsKey(CPU.ActiveProcess))
            {
                ProcessTableEntry entry = m_dProcessTable[CPU.ActiveProcess];
                entry.Done = true;
                if (entry.Console != null)
                    entry.Console.Close();
                entry.EndTime = CPU.TickCount;
            }
            ActivateScheduler();
        }

        public void TimeoutReached()
        {
            ActivateScheduler();
        }

        public void ReadToken(string sFileName, int iTokenNumber, int iProcessId, string sParameterName)
        {
            if (!m_dProcessTable.ContainsKey(iProcessId))
                return;

            ReadTokenRequest request = new ReadTokenRequest();
            request.ProcessId = iProcessId;
            request.TokenNumber = iTokenNumber;
            request.TargetVariable = sParameterName;
            request.Token = null;
            request.FileName = sFileName;

            ProcessTableEntry entry = m_dProcessTable[iProcessId];

            int currentTick = CPU.TickCount;
            int starvationTime = currentTick - entry.LastCPUTime;
            if (starvationTime > entry.MaxStarvation)
            {
                entry.MaxStarvation = starvationTime;
            }

            entry.Blocked = true;

            if (Disk.ActiveRequest == null)
                Disk.ActiveRequest = request;
            else
                m_lReadRequests.Enqueue(request);

            CPU.ProgramCounter = CPU.ProgramCounter + 1;
            ActivateScheduler();
        }

        public void Interrupt(ReadTokenRequest rFinishedRequest)
        {
            double value;
            if (rFinishedRequest.Token == null)
            {
                value = double.NaN;
            }
            else
            {
                if (!double.TryParse(rFinishedRequest.Token, out value))
                {
                    value = double.NaN;
                }
            }

            if (m_dProcessTable.ContainsKey(rFinishedRequest.ProcessId))
            {
                ProcessTableEntry entry = m_dProcessTable[rFinishedRequest.ProcessId];
                entry.AddressSpace[rFinishedRequest.TargetVariable] = value;

                if (!entry.Done)
                {
                    entry.Blocked = false;
                    entry.LastCPUTime = CPU.TickCount;
                    m_spPolicy.AddProcess(rFinishedRequest.ProcessId);
                }
            }

            if (m_lReadRequests.Count > 0)
            {
                ReadTokenRequest nextRequest = m_lReadRequests.Dequeue();
                Disk.ActiveRequest = nextRequest;
            }
            else
            {
                Disk.ActiveRequest = null;
            }

            if (m_spPolicy.RescheduleAfterInterrupt())
            {
                ActivateScheduler();
            }
        }

        private ProcessTableEntry ContextSwitch(int iEnteringProcessId)
        {
            int currentTick = CPU.TickCount;

            ProcessTableEntry outgoingProcess = null;
            if (CPU.ActiveProcess != -1 && m_dProcessTable.ContainsKey(CPU.ActiveProcess))
            {
                outgoingProcess = m_dProcessTable[CPU.ActiveProcess];

                int starvationTime = currentTick - outgoingProcess.LastCPUTime;
                if (starvationTime > outgoingProcess.MaxStarvation)
                {
                    outgoingProcess.MaxStarvation = starvationTime;
                }

                outgoingProcess.ProgramCounter = CPU.ProgramCounter;
                outgoingProcess.Quantum = CPU.RemainingTime;
                outgoingProcess.LastCPUTime = currentTick;
            }

            ProcessTableEntry incomingProcess = m_dProcessTable[iEnteringProcessId];

            CPU.ActiveProcess = incomingProcess.ProcessId;
            CPU.ActiveAddressSpace = incomingProcess.AddressSpace;
            CPU.ActiveConsole = incomingProcess.Console;
            CPU.ProgramCounter = incomingProcess.ProgramCounter;
            CPU.RemainingTime = incomingProcess.Quantum;

            return outgoingProcess;
        }

        public void ActivateScheduler()
        {
            int currentTick = CPU.TickCount;
            foreach (var entry in m_dProcessTable.Values)
            {
                if (!entry.Blocked && !entry.Done && entry.ProcessId != CPU.ActiveProcess)
                {
                    int starvationTime = currentTick - entry.LastCPUTime;
                    if (starvationTime > entry.MaxStarvation)
                    {
                        entry.MaxStarvation = starvationTime;
                    }
                }
            }

            int iNextProcessId = m_spPolicy.NextProcess(m_dProcessTable);
            if (iNextProcessId == -1)
            {
                Console.WriteLine("All processes terminated or blocked.");
                CPU.Done = true;
            }
            else
            {
                ContextSwitch(iNextProcessId);
            }
        }

        public double AverageTurnaround()
        {
            double total = 0;
            int count = 0;
            foreach (var entry in m_dProcessTable.Values)
            {
                if (entry.EndTime > 0 && entry.ProcessId != m_iIdleProcessId)
                {
                    total += (entry.EndTime - entry.StartTime);
                    count++;
                }
            }
            return (count == 0) ? 0 : total / count;
        }

        public int MaximalStarvation()
        {
            int maxStarvation = 0;
            int currentTick = CPU.TickCount;

            foreach (var entry in m_dProcessTable.Values)
            {
                if (entry.ProcessId != m_iIdleProcessId)
                {
                    int totalStarvation = entry.MaxStarvation;

                    if (!entry.Blocked && !entry.Done && entry.ProcessId != CPU.ActiveProcess)
                    {
                        totalStarvation += (currentTick - entry.LastCPUTime);
                    }

                    if (totalStarvation > maxStarvation)
                    {
                        maxStarvation = totalStarvation;
                    }
                }
            }
            return maxStarvation;
        }
    }
}