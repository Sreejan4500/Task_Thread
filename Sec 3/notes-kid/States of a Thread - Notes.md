# Notes: States of a Thread (Kid-Friendly)

*From `2. States of a thread.txt` + `States+of+a+thread.txt`, explained simply.*

---

## 1. A thread can be in different states

**Simple idea:**  
A thread is like a worker that can be doing different things. The .NET documentation has an enumeration of **thread states**, e.g. Unstarted, Running, WaitSleepJoin, Stopped, etc. We don’t need to memorize all of them; the main idea is that the state **changes** over the thread’s life.

- **To a kid:**  
  A worker can be “not started yet,” “working,” “waiting or sleeping,” or “finished.” The state is just a label for what they’re doing right now.

---

## 2. Unstarted

**Simple idea:**  
When you create a thread with `new Thread(Work)` but haven’t called `Start()` yet, its state is **Unstarted**. Every thread starts as Unstarted until it is started.

- **To a kid:**  
  The worker is on the bench. They haven’t been told to start yet.

---

## 3. Running

**Simple idea:**  
After you call `Start()`, the thread moves to **Running** (or a state that means “executing”). While it’s doing work—including when it’s on the line that calls `Thread.Sleep(...)`—it’s still in a running state until the sleep actually blocks it. So right before `Thread.Sleep`, the state is still Running.

---

## 4. WaitSleepJoin

**Simple idea:**  
When the thread is **sleeping** (`Thread.Sleep`), **waiting** for a lock, or **waiting** on another thread (e.g. `Join`), its state is **WaitSleepJoin**. In that state the **thread scheduler** takes the thread off the CPU so it doesn’t use processor time while it waits.

- **To a kid:**  
  The worker is taking a break or waiting in line. They’re not using the desk (CPU) right now, so someone else can use it.

---

## 5. Stopped

**Simple idea:**  
When the thread has finished its method and is no longer running, its state is **Stopped**. So after all the work (and any sleep) is done, the worker thread is in the Stopped state.

---

## 6. How the lesson observes states

**Simple idea:**  
The lesson uses a loop that creates threads, starts them, and later prints each thread’s `ThreadState`:

```csharp
Thread[] threads = new Thread[10];
for (int i = 0; i < 10; i++)
{
    threads[i] = new Thread(Work);
    threads[i].Name = $"Thread {i}";
    Console.WriteLine($"{threads[i].Name}'s state is {threads[i].ThreadState}"); // Unstarted
    threads[i].Start();
}
Thread.Sleep(3000);
for (int i = 0; i < 10; i++)
    Console.WriteLine($"{threads[i].Name}'s state is {threads[i].ThreadState}"); // WaitSleepJoin
// ... after threads finish ...
// Then: Stopped
```

So: before Start → Unstarted; shortly after Start (while in Work) → Running; while sleeping → WaitSleepJoin; when done → Stopped.

---

## 7. Don’t use thread state to synchronize

**Simple idea:**  
The documentation says: **do not use thread state to synchronize your program.** The state is for **debugging and logging** only. Your code should not say “if state is X then do Y” to coordinate threads; use proper tools (locks, events, etc.) instead.

- **To a kid:**  
  The “state” label is so we can see what’s going on when we’re debugging. We don’t use it as the rule for when workers are allowed to do things; we use proper signals and locks for that.

---

## 8. Can we see state in the debugger?

**Simple idea:**  
In some setups, the Threads window doesn’t show a ThreadState column, and in the Immediate Window `Thread.CurrentThread.ThreadState` might say “cannot evaluate because the code is optimized.” So the reliable way to observe state in the lesson is **logging** (e.g. `Console.WriteLine` with the state) rather than relying on the debugger’s state display.

---

## Quick recap (kid version)

| Thing | Simple idea |
|-------|-------------|
| **Unstarted** | Thread created but not started yet. |
| **Running** | Thread is executing (including right before Sleep). |
| **WaitSleepJoin** | Thread is sleeping, waiting for a lock, or Join; not using CPU. |
| **Stopped** | Thread has finished its work. |
| **Use state for** | Debugging and logging only, not for synchronizing threads. |

---

*Same style as the other kid notes; from the States of a thread video and code.*
