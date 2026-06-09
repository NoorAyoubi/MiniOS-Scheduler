using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.IO;

namespace Scheduling
{
    class Program
    {
        static void Example1(OperatingSystem os)
        {
            Console.WriteLine("\n=== Example 1: Basic processes (a.code + b.code) ===");
            for (int i = 0; i < 3; i++)
            {
                os.CreateProcess("a.code");
                os.CreateProcess("b.code");
            }
        }

        static void Example2(OperatingSystem os)
        {
            Console.WriteLine("\n=== Example 2: File reading processes (ReadFile1.code + ReadFile2.code) ===");
            for (int i = 0; i < 3; i++)
            {
                os.CreateProcess("ReadFile1.code");
                os.CreateProcess("ReadFile2.code");
            }
        }

        static void Example3(OperatingSystem os)
        {
            Console.WriteLine("\n=== Example 3: Processes with yield (c.code + d.code) ===");
            for (int i = 0; i < 3; i++)
            {
                os.CreateProcess("c.code");
                os.CreateProcess("d.code");
            }
        }

        static void Example4(OperatingSystem os)
        {
            Console.WriteLine("\n=== Example 4: Priority processes (c.code + d.code with priorities) ===");
            for (int i = 0; i < 3; i++)
            {
                os.CreateProcess("c.code", i);
                os.CreateProcess("d.code", i + 1);
            }
        }

        static void RunWithPolicy(SchedulingPolicy policy, string policyName, Action<OperatingSystem> example, string exampleName)
        {
            Console.WriteLine($"\n{"=".PadRight(70, '=')}");
            Console.WriteLine($"Policy: {policyName} | Example: {exampleName}");
            Console.WriteLine($"{"=".PadRight(70, '=')}");

            Disk disk = new Disk();
            CPU cpu = new CPU(disk);
            cpu.Debug = false;
            OperatingSystem os = new OperatingSystem(cpu, disk, policy);

            example(os);

            os.ActivateScheduler();
            cpu.Execute();

            Console.WriteLine($"\n--- Results for {policyName} ({exampleName}) ---");
            Console.WriteLine($"Average Turnaround Time: {os.AverageTurnaround():F2}");
            Console.WriteLine($"Maximal Starvation Time: {os.MaximalStarvation()}");
            Console.WriteLine($"Total CPU Tick Count: {cpu.TickCount}");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            // Set the working directory to the "Code files" dir
            if (Directory.Exists("../../../Code files"))
                Directory.SetCurrentDirectory("../../../Code files");
            else if (Directory.Exists("Code files"))
                Directory.SetCurrentDirectory("Code files");
            else
                Console.WriteLine("Warning: Code files directory not found!");

            Console.WriteLine("==============================================================");
            Console.WriteLine("Process Scheduling Simulation - Complete Test Suite");
            Console.WriteLine("==============================================================");

            var testScenarios = new List<Tuple<SchedulingPolicy, string, Action<OperatingSystem>, string>>()
            {
                // FCFS Tests
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new FirstComeFirstServedPolicy(), "FCFS", Example1, "Example1"),
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new FirstComeFirstServedPolicy(), "FCFS", Example2, "Example2"),
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new FirstComeFirstServedPolicy(), "FCFS", Example3, "Example3"),
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new FirstComeFirstServedPolicy(), "FCFS", Example4, "Example4"),

                // Round Robin Tests (Quantum = 3)
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new RoundRobinPolicy(3), "RoundRobin(Q=3)", Example1, "Example1"),
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new RoundRobinPolicy(3), "RoundRobin(Q=3)", Example2, "Example2"),
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new RoundRobinPolicy(3), "RoundRobin(Q=3)", Example3, "Example3"),
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new RoundRobinPolicy(3), "RoundRobin(Q=3)", Example4, "Example4"),

                // Prioritized Scheduling Tests (Quantum = 4)
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new PrioritizedScheduling(4), "Prioritized(Q=4)", Example1, "Example1"),
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new PrioritizedScheduling(4), "Prioritized(Q=4)", Example2, "Example2"),
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new PrioritizedScheduling(4), "Prioritized(Q=4)", Example3, "Example3"),
                Tuple.Create<SchedulingPolicy, string, Action<OperatingSystem>, string>(new PrioritizedScheduling(4), "Prioritized(Q=4)", Example4, "Example4"),
            };

            foreach (var scenario in testScenarios)
            {
                RunWithPolicy(scenario.Item1, scenario.Item2, scenario.Item3, scenario.Item4);
            }

            Console.WriteLine("==============================================================");
            Console.WriteLine("All tests completed successfully.");
            Console.WriteLine("==============================================================");
        }
    }
}