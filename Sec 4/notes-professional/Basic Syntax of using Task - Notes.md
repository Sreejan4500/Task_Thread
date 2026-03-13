# Notes: Basic Syntax of Using Task (Professional)

*From transcript "2. Basic Syntax of using Task".*

---

## 1. Task vs Thread: same pattern

**From the transcript:**  
> “It's going to be really really similar to using thread … the syntax is exactly the same.”

- **Thread:** Create a `Thread`, assign a method (delegate), call `thread.Start()`.
- **Task:** Create a `Task`, assign an **`Action`** delegate (void method, no parameters), call `task.Start()`.
- Conceptually identical: declare the unit of work, give it a delegate, start it. The work then runs asynchronously (typically on a thread-pool thread for `Task`).

---

## 2. Basic Task syntax: new Task + Start

- **`Task`** (from `System.Threading.Tasks`) represents a unit of work that can be run asynchronously.
- Constructor: **`new Task(Action action)`** — `Action` is a delegate for `void Method()` with no parameters. You pass the method (or lambda) that does the work.
- **`task.Start()`** schedules the task for execution. Like `thread.Start()`, it is **non-blocking**: the calling thread continues immediately.

**From the transcript:**  
> “Instead of using the thread class, we just use the task class … it takes an action which is a delegate … and then we say task dot start. So nothing fancy. Actually, the syntax is exactly the same.”

Example:

```csharp
void Work() => Console.WriteLine("I love programming!");

Task task = new Task(Work);
task.Start();

Console.ReadLine();  // so we see the result before the program exits
```

- The work runs asynchronously; use something like `Console.ReadLine()` (or later, `await` / `Wait()`) if you need to observe the result or keep the process alive.

---

## 3. Task.Run: create and start in one step

- **`Task.Run(Action action)`** creates a task and **starts it immediately**. No need to call `Start()` separately. Commonly used for “run this on the thread pool.”
- Semantically equivalent to `new Task(action); task.Start();` but shorter and is the preferred way for fire-and-forget or when you will later await or attach continuations.

**From the transcript:**  
> “Another way to run task, which is my favorite way … you can just say Task.run. And in here it takes a delegate just like before … it does exactly the same thing … It's just that when you do it this way, it doesn't warn you.”

Example:

```csharp
Task.Run(() => Console.WriteLine("I love programming!"));
```

- **Return value:** `Task.Run` **returns** a **`Task`** object. That reference lets you wait for completion, handle exceptions, chain continuations, or check status.

---

## 4. “Call is not awaited” warning

- When you call `Task.Run(...)` (or any method that returns `Task`) without **await** or **`.Wait()`**, the compiler may warn: the call is not awaited, so the current method continues before the task completes.
- For **intentionally fire-and-forget** work (run asynchronously, don’t wait), that’s expected. The warning is a reminder; you can suppress it or leave it until you introduce `async`/`await` and proper waiting in later lessons.

**From the transcript:**  
> “It's telling you that you're not waiting for it to complete because this call is not awaited … So that is something we're going to talk about in the future. So don't worry about the green screen line.”

---

## 5. The Task object: what you get back

- **`Task.Run(...)`** returns a **`Task`** instance. Holding that reference gives you:
  - **`Wait()`** — block until the task completes.
  - **`ContinueWith(...)`** — run code when the task completes.
  - **`Id`** — task identifier.
  - **`Status`**, **`IsCompleted`**, **`IsCompletedSuccessfully`** — completion state.
  - **`Exception`** — exception(s) if the task faulted.

**From the transcript:**  
> “The run method actually returns a task object … you can access a lot of different things in it. You can see we have continuewith right. And we have exception. We can get a waiter, we can access the ID of the task, we can check the states … whether it's completed or not … And we can wait. So task can encapsulate a lot of functionalities which we're going to talk about in the future lessons.”

- So: **Task** is not only “run something”; it’s a handle for waiting, composition, and observation.

---

## Summary table

| Concept | Role |
|--------|------|
| **`new Task(Action)`** | Create a task with a delegate (void, no args); does not start it. |
| **`task.Start()`** | Schedule the task; non-blocking; same idea as `thread.Start()`. |
| **`Task.Run(Action)`** | Create and start a task in one call; returns a `Task`; preferred for “run this on the side.” |
| **Action** | Delegate for `void Method()` with no parameters; the “work” the task runs. |
| **Not awaited** | Calling `Task.Run` without `await` or `.Wait()` triggers a warning; normal for fire-and-forget. |
| **Returned Task** | Use for `Wait()`, `ContinueWith`, `Status`, `Exception`, etc., covered in later lessons. |

---

*Same structure as other professional notes in this section; one file per transcript/topic.*
