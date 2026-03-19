# Notes: Returning Result from a Task (Kid-Friendly)

*From `5. Returning result from a task.txt`, explained simply.*

---

## 1. Task can “return” a value

**Simple idea:**  
With **Thread** we had to use a **shared variable** to get a result back. With **Task** we can get a result **built in**: if the work you give the task **returns** a value (e.g. an int or string), you get a **Task&lt;T&gt;** (e.g. **Task&lt;int&gt;**). Then you can read the **Result** property to get that value. So returning from a task is much easier than with a thread.

- **To a kid:**  
  With a thread, the worker had to put the answer in a shared box. With a task, the task itself “holds” the answer, and we can ask for it with **Result**.

---

## 2. How it looks in code

**Simple idea:**  
If your delegate returns nothing (void), you get a plain **Task**. If it returns something (e.g. **int**), you get **Task&lt;int&gt;**:

```csharp
Task<int> task = Task.Run(() =>
{
    int result = 0;
    // ... do work, e.g. add numbers ...
    return result;
});
// Later:
Console.WriteLine($"The result is {task.Result}");
```

So you use **Task.Run** with a delegate that **returns** a value; the type becomes **Task&lt;T&gt;** and **task.Result** gives you that value (after the task has finished).

- **To a kid:**  
  We say “run this job and bring back a number.” The task is like an envelope that will later contain the number. **Result** is when we open the envelope (and we might have to wait until it’s there).

---

## 3. Result and blocking

**Simple idea:**  
Reading **task.Result** **blocks** the current thread until the task has finished. So it’s like **Thread.Join** plus “give me the return value.” We’ll see later that **ContinueWith** and **await** let us use the result **without** blocking the main thread. For now, **Result** is the simple way to get the value; just know it waits until the task is done.

- **To a kid:**  
  When we say “give me the result,” we stand and wait until the task is done and the answer is ready. So the main program can “pause” there until the result is available.

---

## Quick recap (kid version)

| Thing | Simple idea |
|-------|-------------|
| **Task&lt;T&gt;** | A task that “returns” a value of type T. |
| **Result** | The value the task returned; reading it blocks until the task finishes. |
| **Easier than Thread** | No shared variable; the task holds the result. |
| **Blocking** | Using **Result** (or **Wait**) makes the caller wait until the task completes. |

---

*Same style as the other kid notes; from the Returning result from a task video.*
