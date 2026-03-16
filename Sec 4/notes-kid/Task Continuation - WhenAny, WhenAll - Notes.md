# Notes: Task Continuation — WhenAny, WhenAll (Kid-Friendly)

*From `8. Task Continuation - WhenAny, WhenAll.txt`, explained simply.*

---

## 1. The problem: many tasks, then one summary

**Simple idea:**  
In the divide-and-conquer example we had **several tasks** (e.g. each summing a part). We need to **wait for all of them** to finish, then do one more step (e.g. add their results). If we use **t1.Result + t2.Result + ...** we block on each Result. We want a way to say: “when **all** these tasks are done, start a **new** task that combines the results—and don’t block the main thread.” That’s **Task.WhenAll**.

- **To a kid:**  
  Several workers are each doing a piece of the job. We want: “when **everyone** is done, one worker adds up all the answers.” WhenAll is “when everyone is done, do this next.”

---

## 2. WhenAll — one task that completes when all finish

**Simple idea:**  
**Task.WhenAll(task1, task2, task3, ...)** (or an array) **returns a new task**. That new task completes only when **all** the given tasks have completed. So we can do:

```csharp
Task.WhenAll(tasks).ContinueWith(t => {
    // t.Result here is the array of results (e.g. int[])
    int sum = t.Result.Sum();
    Console.WriteLine($"The summary is {sum}");
});
```

So we don’t block the main thread; we create a “when all are done” task and attach a continuation. The continuation runs on a pool thread and gets the results (e.g. **t.Result** as an array). The video shows “This is the end of the program” printing **first**, then the summary—so WhenAll + ContinueWith is non-blocking.

- **To a kid:**  
  WhenAll is like a referee who only blows the whistle when every runner has crossed the line. Then the next job (add up the results) runs.

---

## 3. WhenAny — when any one finishes

**Simple idea:**  
**Task.WhenAny(task1, task2, ...)** returns a **task that completes when the first** of the given tasks completes. So you get a Task that wraps the “winner”—useful when you only need one result (e.g. first response from several servers). The return type is **Task&lt;Task&gt;** (or Task&lt;Task&lt;T&gt;&gt;) because the **result** of WhenAny is the **task that finished**. We’ll see **Unwrap** in the next note for chaining with that. For “wait for all and then continue,” we mostly use **WhenAll**.

- **To a kid:**  
  WhenAny is “as soon as **any one** of these is done, I’m done.” Like the first person to hand in their test—you get that one task back.

---

## 4. Chaining ContinueWith with WhenAll

**Simple idea:**  
We do **Task.WhenAll(tasks).ContinueWith(...)**. So the continuation runs only after every task in the array has finished. Inside the continuation, **t.Result** is the array of results (e.g. **int[]** for Task&lt;int&gt;[]). So we can sum them, or process them, without ever blocking the main thread with Wait or Result on the main thread.

---

## Quick recap (kid version)

| Thing | Simple idea |
|-------|-------------|
| **WhenAll(tasks)** | A task that completes when **all** given tasks complete. |
| **WhenAny(tasks)** | A task that completes when **any one** of the given tasks completes. |
| **Use with ContinueWith** | WhenAll(...).ContinueWith(t => use t.Result) — non-blocking. |
| **t.Result for WhenAll** | Array of results (e.g. int[]) from all tasks. |
| **WhenAny result** | The task that finished first (need Unwrap for chaining sometimes). |

---

*Same style as the other kid notes; from the WhenAny, WhenAll video.*
