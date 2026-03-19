# Notes: Returning Result from a Task (Professional)

*From `5. Returning result from a task.txt`.*

---

## 1. Task&lt;T&gt; and Result

When the delegate passed to **Task.Run** (or the **Task&lt;T&gt;** constructor) **returns a value** of type **T**, the API returns **Task&lt;T&gt;** instead of **Task**. The **Result** property of **Task&lt;T&gt;** returns that value. So there is first-class support for “returning” a result from asynchronous work, without shared variables or explicit synchronization for the return value.

---

## 2. Syntax

- **Task** (non-generic): delegate returns **void** (Action).
- **Task&lt;T&gt;** (generic): delegate returns **T** (Func&lt;T&gt;).  
  Example: **Task.Run(() => 100)** yields **Task&lt;int&gt;**; **task.Result** is **100** (once the task has completed).

---

## 3. Blocking

**Result** is a blocking property: the calling thread is blocked until the task has completed. So **task.Result** is equivalent in effect to **task.Wait()** plus reading the return value. For non-blocking consumption of the result, use **ContinueWith** or **await** (covered later).

---

## Summary table

| Concept | Role |
|--------|------|
| **Task&lt;T&gt;** | Task that produces a result of type T. |
| **Result** | Blocking property that returns T once the task has completed. |
| **Comparison to Thread** | No shared variable needed; task holds the result. |

---

*Professional summary of the Returning result from a task video.*
