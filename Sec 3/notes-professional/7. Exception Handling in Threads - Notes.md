# Notes: Exception Handling in Threads (Professional)

*From `7. Exception Handling in Threads.txt` + `Thread+Exceptions+Handling.txt`.*

---

## 1. Separate call stacks

Each thread has its **own call stack**. Exceptions propagate **along that thread’s stack** only. They do **not** propagate to the thread that started the worker (the “calling” thread). So a **try-catch** in the main thread around **thread.Start()** or **thread.Join()** does **not** catch exceptions thrown inside the worker thread. The caller and the worker have independent stacks; the exception is not marshaled to the caller.

---

## 2. Unhandled exceptions and process termination

An unhandled exception in **any** thread can terminate the **process** (or application domain). So even though the main thread cannot catch the worker’s exception, the application may still crash. The exception is observed at the runtime/application level (e.g. unhandled exception handler), not in the caller’s catch block.

---

## 3. Handle inside the worker thread

To handle exceptions **in a controlled way**, put **try-catch** inside the **worker’s entry method** (or in the code path that runs on that thread). There you can log, set flags, or add the exception to a shared collection. The main thread will not receive the exception object in its catch; it can only observe side effects (e.g. a shared list of exceptions) after synchronizing with the worker (e.g. **Join**).

---

## 4. Sharing exceptions with the caller: shared collection

To report exceptions back to the main (or another) thread, use a **shared collection** (e.g. **List&lt;Exception&gt;**). In the worker’s catch block, add the exception to the list. Because multiple threads may add concurrently, access to the list must be **synchronized** (e.g. **lock (lockObject) { exceptions.Add(ex); }**). After **Join()**ing all workers, the main thread can iterate the list and handle or log each exception. The lesson code demonstrates this with two threads that throw, catch, and add to a locked list; the main thread then prints each message.

---

## 5. Multiple exceptions

If one thread can throw multiple times (e.g. in a loop) or you have many threads, the same pattern applies: each catch adds to the shared collection under the lock. The main thread processes the collection after all workers have completed. So you can collect and handle multiple exceptions from multiple threads in one place.

---

## 6. Task and exception handling

The lesson notes that **Task** provides built-in exception handling: exceptions from the task delegate can be observed via **task.Result** (which rethrows), **task.Exception** (aggregate), or **await** (which propagates). With **Thread**, the shared-collection + lock pattern is the standard approach for reporting exceptions to the caller.

---

## Summary table

| Concept | Role |
|--------|------|
| **Call stack** | Per thread; exceptions do not cross to the calling thread. |
| **Main thread catch** | try-catch around Start/Join does not catch worker exceptions. |
| **Handle in worker** | Use try-catch inside the thread’s method to handle or collect. |
| **Shared list + lock** | Collect exceptions in a locked list; main thread reads after Join. |
| **Task** | Provides first-class exception propagation (Result, Exception, await). |

---

*Professional summary of the Exception handling in threads video and code.*
