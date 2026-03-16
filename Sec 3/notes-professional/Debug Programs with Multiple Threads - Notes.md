# Notes: Debug Programs with Multiple Threads (Professional)

*From `1. Debug programs with multiple threads.txt`.*

---

## 1. All threads pause together

When execution is paused in the debugger (breakpoint or manual pause), **all threads** are suspended, not just the current one. This is a fundamental property of the debugger: you can inspect any thread’s call stack and state while everything is stopped.

---

## 2. Threads window (Debug → Windows → Threads)

The **Threads** window lists all threads in the process. The count will exceed the number of threads you created because **system threads** (CLR, finalization, etc.) are included. You can:

- **Flag** threads (icon or right-click → Flag) and use **“Show Flagged Threads Only”** to reduce clutter.
- **Double-click** a thread to switch the debugger’s current context to that thread; the Call Stack and code view then reflect that thread.
- Use the **Immediate Window** and evaluate `Thread.CurrentThread.ManagedThreadId` to confirm which thread is current.

---

## 3. Call stack per thread

Each thread has its own **call stack**. After double-clicking a thread in the Threads window, the Call Stack window and the source view show that thread’s execution point. Thread markers in the margin indicate the current thread at a given location.

---

## 4. Naming threads

Assigning `thread.Name` (e.g. `thread.Name = $"Thread {i}"`) makes the Threads window and **Parallel Stacks** easier to read. Names appear in the thread list and in group labels.

---

## 5. Parallel Stacks (Debug → Windows → Parallel Stacks)

**Parallel Stacks** shows threads grouped by call stack. You get a visual grouping (e.g. main thread, your worker threads, system threads). For **deadlocks**, the view can show warnings (e.g. red flags) and which thread is waiting on which (e.g. “waiting on lock owned by thread &lt;id&gt;”). You can switch context by double-clicking a frame. The Threads window can also show “waiting on lock owned by thread &lt;id&gt;” in the thread’s location; you can search/filter by that ID to find the other thread.

---

## 6. Freeze and thaw

In the Threads window you can **freeze** (suspend) individual threads and **thaw** (resume) them. A frozen thread does not run when you press Continue; only non-frozen threads execute. This allows you to control execution order (e.g. let worker threads finish while the main thread stays frozen, or the reverse) to reproduce or inspect specific interleavings.

---

## Summary table

| Tool / concept | Role |
|----------------|------|
| **Pause** | Suspends all threads so any can be inspected. |
| **Threads window** | List of all threads; flag/filter and switch context by double-click. |
| **Parallel Stacks** | Grouped view of call stacks; can show deadlock and lock-wait info. |
| **Thread names** | Set `Name` for easier identification in debugger windows. |
| **Freeze / Thaw** | Suspend or resume a single thread to control scheduling during debug. |

---

*Professional summary of the Debug multi-thread programs video.*
