# Notes: Task Continuation — WhenAny, WhenAll (Professional)

*From `8. Task Continuation - WhenAny, WhenAll.txt`.*

---

## 1. Problem: multiple tasks, then aggregate

When multiple tasks run in parallel (e.g. divide-and-conquer), the next step often depends on **all** of them completing. Using **t1.Result + t2.Result + ...** blocks the caller on each **Result**. To avoid blocking the main thread, we want a **single** task that completes when **all** are done, and then run a continuation that aggregates results. **Task.WhenAll** provides that.

---

## 2. WhenAll

**Task.WhenAll(IEnumerable&lt;Task&gt;)** or **Task.WhenAll(params Task[])** returns a **Task** that completes when **every** task in the set has completed. Overloads for **Task&lt;T&gt;[]** return **Task&lt;T[]&gt;** so the **Result** of the returned task is the **array of results** in the same order as the input tasks. So we can write:

**Task.WhenAll(tasks).ContinueWith(t => { var results = t.Result; /* aggregate */ });**

The continuation runs only after all tasks have completed; **t.Result** is the array of results (e.g. **int[]** for **Task&lt;int&gt;[]**). The calling thread is not blocked (except if it later waits on the WhenAll task or its continuation).

---

## 3. WhenAny

**Task.WhenAny(...)** returns a **Task** that completes when **any one** of the supplied tasks completes. The **result** of that returned task is the **task that completed** (so the type is **Task&lt;Task&gt;** or **Task&lt;Task&lt;T&gt;&gt;**). So we get “which task finished first” rather than “all results.” Useful for scenarios like “first response wins” (e.g. multiple servers). For chaining, we may need **Unwrap()** to flatten **Task&lt;Task&lt;T&gt;&gt;** to **Task&lt;T&gt;** so the next continuation receives the inner task’s result.

---

## 4. Chaining with ContinueWith

The lesson demonstrates **Task.WhenAll(tasks).ContinueWith(...)** so that the continuation receives the array of results and computes the sum (or other aggregation) without the main thread blocking. “This is the end of the program” is printed first, then the summary—confirming non-blocking behavior.

---

## Summary table

| Method | Role |
|--------|------|
| **WhenAll(tasks)** | Returns a task that completes when all given tasks complete; Result is T[] for Task&lt;T&gt;[]. |
| **WhenAny(tasks)** | Returns a task that completes when any one task completes; Result is the completed task. |
| **ContinueWith** | Chain WhenAll(...).ContinueWith(t => use t.Result) for non-blocking aggregation. |
| **WhenAny result** | Task&lt;Task&gt; or Task&lt;Task&lt;T&gt;&gt;; use Unwrap for flat Task&lt;T&gt; when chaining. |

---

*Professional summary of the WhenAny, WhenAll video.*
