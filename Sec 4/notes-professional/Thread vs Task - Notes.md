# Notes: Thread vs Task (Professional)

*From `3. Thread vs Task.txt`.*

---

## 1. Task as promise; Thread as execution unit

- **Task** is a **promise** (future) of work to be completed sometime later; it does not necessarily imply a thread. It is the abstraction used for **asynchronous** programming.
- **Thread** is the basic unit of execution on the CPU; it is the low-level construct that runs code.

Tasks **often** use a thread (typically a thread-pool thread) to do the work, but the abstraction is “completion sometime in the future,” not “a thread.” So Task is higher-level and can accommodate I/O-based or other completion without a dedicated thread.

---

## 2. Microsoft recommendation: prefer Task

Microsoft recommends **Task** over **Thread** not because it is newer but because it is a **higher-level abstraction** that provides:

- **Thread-pool usage by default** — no manual thread creation.
- **Return values** — `Task<T>` and `Result` (or await) without shared variables and associated synchronization/race issues.
- **Continuations** — e.g. **ContinueWith**, **WhenAll**, **WhenAny** to chain or combine work without blocking.
- **Structured exception handling** — exceptions are captured in the task (**Exception** property, **AggregateException**) rather than escaping to the application domain unseen.
- **async/await** — enables synchronous-looking code for asynchronous flow.
- **Synchronization context** — supports marshaling back to the UI or other context (thread affinity).

With **Thread**, achieving the same behavior requires manual handling of pooling, results, continuations, exceptions, and context.

---

## 3. Scope of this section

The video states that the section will cover: task and thread pool, returning values, **Wait** / **WaitAll** / **Result**, **ContinueWith**, **WhenAll** / **WhenAny**, exception handling, synchronization (same primitives as threads), and cancellation (CancellationToken). These are the “out of the box” benefits of Task.

---

## Summary table

| Concept | Role |
|--------|------|
| **Task** | Promise of completion; async abstraction; may or may not use a thread. |
| **Thread** | Execution unit; runs code on the CPU. |
| **Prefer Task** | Higher-level; built-in results, continuations, exceptions, pool, await. |
| **This section** | Pool, Result, Wait/WaitAll, ContinueWith, WhenAll/WhenAny, exceptions, sync, cancellation. |

---

*Professional summary of the Thread vs Task video.*
