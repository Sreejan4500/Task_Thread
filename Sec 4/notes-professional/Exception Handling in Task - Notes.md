# Notes: Exception Handling in Task (Professional)

*From `10. Exception Handling in Task.txt`.*

---

## 1. Exceptions are captured, not propagated to caller

Unlike with **Thread**, where an unhandled exception can still terminate the process, **exceptions thrown inside a Task** are **captured** and stored in the task. They do **not** propagate to the thread that started or is waiting on the task at the moment they occur. So the application does not crash at the throw site; the task transitions to **Faulted** and holds the exception(s). The caller sees the failure only when it **observes** the task (e.g. **Wait**, **Result**, **await**, or **Exception** property).

---

## 2. Try-catch around Start/Run does not catch task exceptions

A **try-catch** around **Task.Run(...)** or **task.Start()** does **not** catch exceptions that occur inside the task’s delegate. Those run on another thread (or later); the exception is stored in the task. So handling must be done **inside** the delegate, or by inspecting the task after completion (**Status**, **Exception**, **IsFaulted**).

---

## 3. Task status and Exception property

- **Status**: when the task has thrown, **Status** is **Faulted**.
- **Exception**: of type **AggregateException**; it can contain **multiple** exceptions in **InnerExceptions** (e.g. from multiple operations or from **WhenAll** over several faulted tasks). So we can iterate **task.Exception.InnerExceptions** to handle or log each one.

Checking **task.IsFaulted** and then **task.Exception?.InnerExceptions** is the standard way to handle faults without rethrowing.

---

## 4. WhenAll and multiple exceptions

When using **Task.WhenAll(tasks)** and one or more tasks fault, the task returned by **WhenAll** is also **Faulted**. Its **Exception** aggregates the exceptions from **all** faulted tasks (in **InnerExceptions**). So a single continuation on **WhenAll(...).ContinueWith(t => ...)** can inspect **t.Exception.InnerExceptions** and handle every failure from every task.

---

## 5. Wait() and Result rethrow

Calling **task.Wait()** or reading **task.Result** causes the **stored** exception(s) to be **rethrown** in the calling thread. They are thrown as **AggregateException** (with the actual exceptions in **InnerExceptions**). So the caller can catch **AggregateException** and handle or unwrap. This is “observing” the task and triggering the throw at observation time.

---

## 6. Continuation only when not faulted

**ContinueWith** has overloads that take **TaskContinuationOptions**. **NotOnFaulted** (and similar) ensure the continuation runs only when the antecedent did **not** fault. So we can avoid running follow-up logic when the previous task failed.

---

## 7. await and exception type

With **await**, the task’s exception is also thrown when the task is awaited. The difference: **await** typically throws the **first** inner exception (e.g. **HttpRequestException**), not the **AggregateException**. So **Wait**/ **Result** → **AggregateException**; **await** → first **InnerException**. Both surface the failure; the type differs.

---

## Summary table

| Concept | Role |
|--------|------|
| **Faulted / Exception** | Exceptions stored in task; Status = Faulted; Exception = AggregateException. |
| **InnerExceptions** | Multiple exceptions (e.g. from WhenAll); iterate to handle each. |
| **Try-catch around Run** | Does not catch; handle inside delegate or by inspecting task. |
| **Wait / Result** | Rethrow stored exception as AggregateException. |
| **NotOnFaulted** | Run continuation only when antecedent did not fault. |
| **await** | Throws first inner exception, not AggregateException. |

---

*Professional summary of the Exception handling in Task video.*
