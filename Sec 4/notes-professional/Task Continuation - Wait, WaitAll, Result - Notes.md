# Notes: Task Continuation — Wait, WaitAll, Result (Professional)

*From `6. Task Continuation - Wait, WaitAll, Result.txt`.*

---

## 1. Waiting for completion

Because a task represents work that completes sometime in the future, the caller often needs to **wait** until completion before using the result or proceeding. **Wait()** and **WaitAll(...)** provide blocking wait semantics analogous to **Thread.Join** (single or multiple).

---

## 2. Wait()

**task.Wait()** blocks the **calling thread** until the task has completed (RanToCompletion, Faulted, or Canceled). After **Wait()** returns, the caller can safely read **Result** (for Task&lt;T&gt;) or check **Status** / **Exception**. So **Wait** is the task equivalent of **Thread.Join**.

---

## 3. WaitAll()

**Task.WaitAll(params Task[] tasks)** (and overloads) blocks the calling thread until **all** of the supplied tasks have completed. Used when the next step depends on the completion of several tasks (e.g. gathering results from a divide-and-conquer pattern). The caller is blocked until every task in the set is done.

---

## 4. Result

For **Task&lt;T&gt;**, **task.Result** returns the task’s result value. **Accessing Result also blocks** until the task has completed. So **Result** effectively combines **Wait()** and reading the return value. If the task is faulted, **Result** will throw (typically an **AggregateException** wrapping the task’s exceptions). Using **Result** (or **Wait**) makes the call **synchronous** at that point; for non-blocking flow, use **ContinueWith** or **await**.

---

## 5. When to use continuation instead

If the caller always blocks on **Wait** or **Result**, the benefits of asynchronous execution (e.g. responsive UI, efficient use of threads) are lost at that call site. **ContinueWith** (and later **await**) allow chaining work that runs when the task completes **without** blocking the calling thread. The video introduces **ContinueWith** as the next step for non-blocking continuation.

---

## 6. Real-world note: async APIs

The lesson uses **HttpClient.GetStringAsync(url)**, which returns **Task&lt;string&gt;**. Calling **.Result** on it blocks until the HTTP response is available. Many .NET APIs provide async methods that return **Task**/ **Task&lt;T&gt;**; using them with **Result** or **Wait** is synchronous at that point but still uses the async implementation (which may use I/O completion and thread-pool efficiency). For fully non-blocking flow, **await** or continuations are used.

---

## Summary table

| Method / property | Role |
|-------------------|------|
| **Wait()** | Block until this task completes. |
| **WaitAll(tasks)** | Block until all given tasks complete. |
| **Result** | Block and return the task’s result (Task&lt;T&gt;); throws if faulted. |
| **Blocking** | All of these block the calling thread. |
| **Non-blocking alternative** | ContinueWith, WhenAll(...).ContinueWith(...), await. |

---

*Professional summary of the Task continuation Wait, WaitAll, Result video.*
