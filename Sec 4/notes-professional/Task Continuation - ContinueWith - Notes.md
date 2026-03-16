# Notes: Task Continuation — ContinueWith (Professional)

*From `7. Task Continuation - ContinueWith.txt`.*

---

## 1. Non-blocking continuation

**ContinueWith** schedules a **new task** that runs only **after** the antecedent task has completed. The continuation receives the **antecedent task** as an argument (e.g. **t**), so it can read **t.Result**, **t.Exception**, etc. The **calling thread** is not blocked; the continuation runs on a thread-pool thread (or as configured). So we get “when this task is done, do that next” without blocking the main thread.

---

## 2. Signature and return value

**task.ContinueWith(Func&lt;Task, TResult&gt; continuationFunction)** (and overloads) returns a **new Task** (or **Task&lt;TResult&gt;**). That returned task completes when the continuation has run. So continuations can be chained: **task1.ContinueWith(...).ContinueWith(...)**. The delegate receives the **antecedent task**; reading **t.Result** inside the continuation may block **that** continuation’s thread but not the original caller.

---

## 3. Example: fetch then parse

The video uses two tasks: (1) get JSON from a URL (**GetStringAsync**), (2) **ContinueWith** to parse the JSON and print the first Pokémon’s name and URL. “This is the end of the program” is printed **before** the JSON output, demonstrating that the main thread is not blocked by the two tasks. So the pattern is: start async work, attach a continuation that consumes the result, and let the main thread proceed.

---

## 4. Chaining

Multiple steps can be chained: task1 → ContinueWith → task2 → ContinueWith → task3. Each step runs after the previous one completes; all run without blocking the original caller. Later videos cover **WhenAll** (continue when multiple tasks complete) and **Unwrap** (when the continuation returns a Task and we need to flatten Task&lt;Task&lt;T&gt;&gt;).

---

## Summary table

| Concept | Role |
|--------|------|
| **ContinueWith** | Schedule a task to run when the antecedent completes. |
| **Parameter (t)** | The antecedent task; use t.Result, t.Exception, etc. |
| **Non-blocking** | Caller does not block; continuation runs on pool thread. |
| **Return value** | New Task/Task&lt;T&gt; representing the continuation; enables chaining. |

---

*Professional summary of the ContinueWith video.*
