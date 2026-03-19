# Notes: States of a Thread (Professional)

*From `2. States of a thread.txt` + `States+of+a+thread.txt`.*

---

## 1. ThreadState enumeration

The .NET **ThreadState** enumeration represents the lifecycle and status of a thread. Documented values include (among others): **Unstarted**, **Running**, **WaitSleepJoin**, **Stopped**, and others. The exact semantics are in the documentation; the important point is that state changes as the thread is created, started, blocks, and terminates.

---

## 2. Unstarted

A thread is **Unstarted** from creation until **Start()** is called. So any thread you create with `new Thread(...)` and do not start is in Unstarted state.

---

## 3. Running

After **Start()**, the thread enters a runnable/executing state. The lesson notes that when the thread is about to call **Thread.Sleep**, it is still in a running state at the point of the call; the transition to a waiting state happens when the sleep (or other wait) is actually applied.

---

## 4. WaitSleepJoin

When the thread is blocked due to **Thread.Sleep**, waiting for a **lock**, or **Join** on another thread, its state is **WaitSleepJoin**. In this state the thread scheduler does not schedule the thread for CPU time; it is not consuming processor cycles until the wait condition is satisfied.

---

## 5. Stopped

When the thread’s entry method has completed and the thread is no longer running, its state is **Stopped**. So after all work (and any blocking) is done, the thread transitions to Stopped.

---

## 6. Observing state in the lesson

The lesson uses a **Thread[]** and prints **threads[i].ThreadState** at different times:

- Before **Start()**: Unstarted.
- Shortly after **Start()** (worker in its method): Running.
- After a delay while workers are in **Thread.Sleep**: WaitSleepJoin.
- After workers have completed: Stopped.

So state is observed via **logging** (e.g. Console) at well-defined points, not relied on for synchronization.

---

## 7. Do not use ThreadState for synchronization

The documentation states that **thread state should not be used to synchronize the activities of threads**. It is intended for **debugging and diagnostic** scenarios. Synchronization should use the proper primitives (lock, Monitor, events, etc.), not polling or branching on ThreadState.

---

## 8. Debugger and ThreadState

In optimized builds, the debugger may not show a ThreadState column or may report “cannot evaluate expression because the code of the current method is optimized” for **Thread.CurrentThread.ThreadState**. So for observing state, logging (e.g. Console output) at specific points is more reliable than the debugger’s state display.

---

## Summary table

| State | Meaning |
|-------|---------|
| **Unstarted** | Created but Start() not called. |
| **Running** | Executing (or about to block). |
| **WaitSleepJoin** | Blocked (Sleep, lock wait, Join). |
| **Stopped** | Entry method finished. |
| **Usage** | Debugging/logging only; do not use for synchronization. |

---

*Professional summary of the States of a thread video and code.*
