# Notes: Tasks Synchronization (Professional)

*From `11. Tasks Synchronization.txt`.*

---

## 1. Same primitives as thread synchronization

**Task synchronization is the same as thread synchronization.** Tasks are executed by threads (typically thread-pool threads), so concurrent tasks imply concurrent threads accessing shared state. All synchronization primitives used with **Thread** apply to **Task**:

- **lock** (Monitor)
- **Mutex**
- **ReaderWriterLock** / **ReaderWriterLockSlim**
- **Semaphore** / **SemaphoreSlim**
- **AutoResetEvent** / **ManualResetEvent** (and slim variants)

No new primitives are required; the same rules (critical sections, lock ordering, etc.) apply.

---

## 2. Example: semaphore + lock with Task

The lesson takes the **semaphore**-based example (limit concurrent request processing) and **replaces Thread with Task**. The **semaphore** still limits how many tasks (and thus threads) process requests at once. The **exclusive lock** still protects the **request queue** (e.g. **Queue&lt;T&gt;**), which is not thread-safe. So the change is from **new Thread(...).Start()** to **Task.Run(...)** (or equivalent); the **lock** and **semaphore** usage remain unchanged. Behavior (and thread safety) is preserved.

---

## 3. Why synchronization is still required

Because tasks run on threads, multiple tasks can access the same shared data concurrently. Without synchronization, the same race conditions and data corruption that occur with threads occur with tasks. So when using **Task**, shared mutable state (queues, counters, caches, etc.) must still be protected with the appropriate primitive (lock, semaphore, etc.), exactly as with **Thread**.

---

## Summary table

| Concept | Role |
|--------|------|
| **Task sync = thread sync** | Same locks, monitors, mutexes, semaphores, events. |
| **Replace Thread with Task** | Sync logic unchanged; only the unit of work (Task vs Thread) changes. |
| **Shared data** | Must still be protected when multiple tasks (threads) access it. |

---

*Professional summary of the Tasks Synchronization video.*
