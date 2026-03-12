# Notes: CPU, Thread, and Thread Scheduler (Professional)

*From transcript + OS/systems context*

---

## 1. CPU and execution model

- **CPU** = central processing unit; executes instructions. No CPU ⇒ no execution.
- **Execution unit:** The CPU does **not** schedule or execute “processes” or “applications” directly. The **schedulable unit** is the **thread** (kernel-schedulable entity in OS terms).
- Applications are loaded into memory; to run, their **thread(s)** must be **assigned to a logical CPU** (core or hardware thread). The entity that does this assignment is the **thread scheduler** (OS component).

---

## 2. Thread as the unit of scheduling

**From the transcript:**  
> “The CPU can only process a thread … thread is the basic unit that can run within a CPU, not the application.”

- **Process** = address space + resources (handles, working set, etc.).
- **Thread** = unit of execution within a process; has its own stack, register state, and program counter; shares process memory and resources.
- At any instant, **one logical CPU runs at most one thread**. The OS scheduler decides *which* thread runs on *which* CPU.

So: **application** (process) exists for resource and isolation boundaries; **thread** is what the CPU actually runs.

---

## 3. Thread scheduler (OS component)

- **Thread scheduler** = OS kernel component that:
  - Maintains ready queues (per-CPU and sometimes global),
  - Chooses the next thread to run (scheduling policy),
  - Performs context switches and (on multicore) load balancing.
- **Developer cannot control the scheduler** beyond:
  - **Thread/process priority** (e.g. `SetThreadPriority` on Windows, nice/sched_setscheduler on Linux),
  - **Affinity** (hinting which CPUs a thread may run on),
  - **Yield** (voluntarily giving up the remainder of a time slice).

Policy details (exact time slices, when to preempt, which queue) are OS-dependent and not part of the process’s direct control.

---

## 4. Single core: concurrency via time slicing

- **Single core:** Only one thread is *executing* at a time.
- Scheduler uses **time slicing** (time quantum): each thread runs for a bounded time, then is preempted so others can run.
- Preemption avoids one thread monopolizing the CPU; other threads get turns (fairness, responsiveness).
- **Concurrency** = multiple threads make progress over time by taking turns. **Parallelism** (multiple threads executing at the same instant) requires multiple cores.

---

## 5. Multi-core: parallelism

- **Multiple cores** = multiple logical CPUs. Scheduler can assign **one runnable thread per core** at any moment.
- Then multiple threads **execute in parallel** (true parallelism, not only time-sliced concurrency).
- Scheduler still uses priorities, time slices, and load balancing; it just has more CPUs to place threads on.

---

## 6. Single-threaded vs multi-threaded applications

- **Single-threaded:** One process, one thread (the main/primary thread). All work is serialized on that thread.
- **Multi-threaded:** One process, **multiple** threads. Used for:
  - Overlapping I/O and computation,
  - Using multiple cores for CPU-bound work,
  - Keeping UI responsive (e.g. main thread for UI, worker threads for heavy work).

**From the transcript:**  
To the scheduler, it doesn’t matter whether threads belong to one process or many. It only sees **threads** and assigns them to CPUs according to its policy. So “five threads in the system” might be 3 from App A and 2 from App B—scheduling is thread-centric.

---

## 7. Code-level mental model (no framework specifics)

Conceptually, the flow is:

1. **Process** starts with one **main thread**.
2. That thread (and any others the process creates) are **runnable** and enter the OS **scheduler’s queues**.
3. Scheduler assigns runnable threads to CPUs; when a thread’s quantum expires or it blocks (I/O, lock, etc.), it’s descheduled and another runs.
4. Your code runs when its thread is **scheduled onto a core**; you don’t control *when* that happens, only priorities/affinity/yield.

Example (pseudocode) of “one process, one main thread”:

```text
Process starts
  → OS creates main thread, marks it runnable
  → Scheduler eventually assigns it to a CPU
  → main() runs on that thread
  → When main() returns (and no other threads exist), process exits
```

Multi-threaded (conceptual):

```text
Process starts, main thread runs
  → main thread calls create_thread(worker_fn)
  → OS creates new thread, runnable
  → Scheduler can run main thread on Core 0, worker on Core 1 (if available)
  → Both execute concurrently/in parallel; scheduler preempts and switches as per policy
```

---

## 8. Real-world implications

- **Latency / responsiveness:** Long work on the main thread (e.g. heavy computation or blocking I/O) delays scheduling of that thread and can freeze the UI; offload to worker threads or use async I/O.
- **Priorities:** Use sparingly; misused high priority can starve other threads or the whole system.
- **Predictability:** Scheduler is non-deterministic from the program’s view; use synchronization (locks, wait/signal) for correctness, not reliance on exact scheduling order.

---

## Summary table

| Concept            | Definition / role |
|--------------------|-------------------|
| **CPU**            | Hardware that executes instructions; runs one thread per logical CPU at a time. |
| **Process**        | Address space + resources; container for threads. |
| **Thread**         | Unit of execution; schedulable entity; has stack, PC, registers; shares process memory. |
| **Main thread**    | Initial thread of a process (e.g. where `main()` runs). |
| **Thread scheduler**| OS component that assigns threads to CPUs (priorities, time slices, affinity). |
| **Time slice**     | Max time a thread runs before preemption (policy-dependent). |
| **Single-threaded**| One thread per process. |
| **Multi-threaded** | Multiple threads per process; scheduler sees all threads from all processes. |

---

*Structure: one folder per audience (`notes-kid`, `notes-professional`) under `Sec 1`; one notes file per transcript. New transcripts → new note files in the same layout.*
