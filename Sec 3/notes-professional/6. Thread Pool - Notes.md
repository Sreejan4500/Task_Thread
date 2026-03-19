# Notes: Thread Pool (Professional)

*From `6. Thread Pool.txt` + `ThreadPool.txt`.*

---

## 1. Problem with per-work thread creation

Creating a new **Thread** for each unit of work has cost: thread creation and teardown consume time and memory. Under high load (e.g. many concurrent requests), creating and destroying many threads can hurt performance. A **thread pool** addresses this by maintaining a set of threads that are **reused** for many work items instead of creating a new thread per item.

---

## 2. How the pool works

The pool keeps a number of threads available. When work is **queued**, the pool assigns it to an available thread. When the thread finishes the work item, it returns to the pool and can take the next item. So threads are reused. Benefits: lower creation/teardown overhead and more controlled concurrency. Drawback: the pool has a **maximum** number of threads; once all are busy, additional work must wait in a queue until a thread becomes free.

---

## 3. Per-application; already present

The .NET **thread pool** is **per process** (per application). It is created and managed by the runtime; you don’t instantiate it. Even a simple “Hello World” process has pool threads (visible in the Threads window). So the pool is always there; you choose whether to use it (e.g. via **ThreadPool.QueueUserWorkItem** or **Task**) or to create your own **Thread** instances.

---

## 4. Min and max threads

The pool has configurable **minimum** and **maximum** thread counts. The pool may create threads up to the minimum as needed and can grow up to the maximum under load. Beyond the maximum, new work items wait. **ThreadPool.GetMaxThreads** returns the maximum worker and I/O thread counts; **ThreadPool.GetAvailableThreads** returns how many of those are currently available (not in use). So “active” ≈ max − available. **SetMinThreads** and **SetMaxThreads** exist but are generally not recommended unless you have a specific need; default behavior is usually sufficient.

---

## 5. QueueUserWorkItem

To run work on the pool:

```csharp
ThreadPool.QueueUserWorkItem(callback, state);
```

- **callback** is a **WaitCallback** delegate: `void (object?)`. The method to run must match this signature.
- **state** is the object passed to the callback (can be null). Used to pass input (e.g. the request string) to the callback.

The pool will invoke the callback on a pool thread when one is available. So you “queue” work instead of creating a new thread.

---

## 6. Example: web server using the pool

In the lesson’s web server simulation, the monitoring thread previously created a **new Thread** for each request. That was replaced with **ThreadPool.QueueUserWorkItem(ProcessInput, input)**. **ProcessInput** was changed to accept **object?** and cast to string (or use the state). Behavior is unchanged from the user’s perspective, but threads are now pool threads. **Thread.CurrentThread.IsThreadPoolThread** can be used to confirm execution on a pool thread.

---

## 7. Worker vs I/O threads

The pool distinguishes **worker threads** and **I/O threads** (used for async I/O completion). **GetMaxThreads** and **GetAvailableThreads** report both. For **QueueUserWorkItem**, worker threads are used. Tasks (later in the course) also use the thread pool by default.

---

## Summary table

| Concept | Role |
|--------|------|
| **Thread pool** | Reusable set of threads; reduces create/destroy overhead. |
| **QueueUserWorkItem** | Queue a WaitCallback (+ optional state) to run on a pool thread. |
| **Per process** | Each process has its own pool; created by the runtime. |
| **Min / max** | Pool size is bounded; excess work is queued. |
| **IsThreadPoolThread** | Indicates whether the current thread is from the pool. |

---

*Professional summary of the Thread Pool video and code.*
