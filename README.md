# 🖥️ MiniOS Scheduler Simulation

## 📌 Overview
MiniOS Scheduler is an Operating System simulation project developed in C#.  
It demonstrates how modern operating systems manage processes, CPU scheduling, and I/O operations internally.

The system simulates real OS concepts such as process lifecycle, context switching, scheduling algorithms, and disk interrupts in a controlled environment.

---

## ⚙️ Features

- 🧵 Process creation, execution, and termination
- ⏱️ CPU scheduling algorithms:
  - First Come First Served (FCFS)
  - Round Robin (RR)
  - Priority Scheduling
- 🔄 Context switching simulation
- 💽 Disk I/O with interrupt handling
- ⛔ Process blocking and resuming
- 💤 Idle process handling
- 📊 Performance metrics:
  - Turnaround time
  - Starvation analysis

---

## 🧠 Key Concepts Implemented

This project simulates core Operating System concepts:

- Process Control Block (PCB)
- Ready / Blocked / Running states
- CPU scheduling policies
- Interrupt-driven I/O
- Time slicing (quantum-based execution)
- Starvation tracking

---

## 🏗️ Architecture

The system is composed of the following main components:

- **CPU** → Executes instructions and manages process state
- **OperatingSystem** → Core scheduler and process manager
- **SchedulingPolicy** → Defines scheduling behavior (FCFS / RR / Priority)
- **Disk** → Simulates I/O requests with interrupts
- **ProcessTableEntry** → Represents each process in the system

---

## 📊 Metrics

The simulator calculates:

- ⏳ Turnaround Time
- 📉 Maximum Starvation per process
- 📈 System-level performance comparison between policies

---

## 🧪 How It Works

1. Processes are created and added to the ready queue
2. Scheduler selects next process based on policy
3. CPU executes process instructions line by line
4. I/O requests trigger blocking and disk interrupts
5. Scheduler resumes execution when processes become ready

---

## 🚀 How to Run

1. Open the solution file:  Scheduling.sln
   
2. Build the project using Visual Studio

3. Run the program

---

## 🎯 Educational Purpose

This project was developed as part of Operating Systems coursework to:

- Understand CPU scheduling mechanisms
- Simulate process management internally
- Learn how interrupts and I/O affect execution
- Compare scheduling algorithms in practice

---

## 🧑‍💻 Technologies Used

- C#
- Object-Oriented Programming (OOP)
- Operating Systems Concepts
- Simulation-based design

---

## 📌 Author

Developed as an academic systems programming project focusing on OS internals and scheduling simulation.
