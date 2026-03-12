# Notes: Basic Syntax to Start a Thread (Professional)

*From transcript "Basic Syntax to Start a Thread" + Basic+Syntax code.*

---

## 1. Single-threaded baseline: method invocation

- A program performs work by invoking **methods**. In a single-threaded program, all work runs on the **main thread**.
- Example: a method that writes the current thread’s ID:

  ```csharp
  void WriteThreadId()
  {
      Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
  }
  ```

- Calling `WriteThreadId()` from `Main` runs that logic on the main thread. The main thread’s ID is often 1 in a simple console app.
- **`Thread.CurrentThread`** = the thread that is currently executing the code. **`ManagedThreadId`** is the CLR-assigned ID (useful for debugging and demos).

---

## 2. Starting a new thread with the `Thread` class

**From the transcript:**  
> “Starting a thread, you declare the thread, you assign a task to the thread, and now you're going to say thread one dot start.”

- **`System.Threading.Thread`** represents an OS thread. To use it:
  1. **Declare** a `Thread` and pass a **delegate** (the method to run when the thread starts).
  2. **Start** the thread with **`Start()`** so the OS scheduler can run it.

- The constructor takes a **`ThreadStart`** delegate (a method with signature `void Method()`—no parameters, no return value). You pass the **method name** (no parentheses).

- Minimal example:

  ```csharp
  Thread thread1 = new Thread(WriteThreadId);  // assign task (method)
  thread1.Start();                              // kick off the thread
  ```

- Once **`Start()`** is called, the method runs on a **new thread**. That thread gets its own **`ManagedThreadId`** (e.g. 7, 8), so calling `WriteThreadId()` from the main thread prints 1, and from the new thread prints another ID.

- **Non-blocking:** `thread1.Start()` returns immediately; it does **not** wait for the thread to finish. The main thread and the new thread run **concurrently**.

---

## 3. Running the same task on multiple threads

- You can create several `Thread` instances and give each the **same** method. Each runs on its own thread with its own ID.

  ```csharp
  Thread thread1 = new Thread(WriteThreadId);
  Thread thread2 = new Thread(WriteThreadId);

  thread1.Start();
  thread2.Start();
  ```

- **Order of execution is not guaranteed.** The **thread scheduler** (OS) decides which thread runs when and for how long. One run might show thread 8 finishing all 100 iterations before thread 7; another run might show them interleaved.

---

## 4. Observing the scheduler: longer work and `Thread.Sleep`

- If the task is very short (e.g. 100 quick loops), one thread may complete before the other gets much CPU time, so output can look mostly serial.
- **`Thread.Sleep(milliseconds)`** suspends the **current** thread for (at least) the given time. Used in demos to make the task longer so the scheduler **preempts** the thread and switches to others.

  ```csharp
  for (int i = 0; i < 100; i++)
  {
      Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
      Thread.Sleep(50);  // 50 ms per iteration → visible interleaving
  }
  ```

- With sleep, you see output from multiple threads **interleaved** (e.g. 7, 8, 7, 8, 7, 7, 8…). The exact order depends on time slices and scheduler policy; sometimes one thread prints twice in a row because it got a longer slice.

---

## 5. Blocking vs non-blocking on the main thread

- Work executed **directly** on the main thread (e.g. `WriteThreadId();` in `Main`) **blocks** the main thread until it finishes. Any code after it (e.g. `thread1.Start(); thread2.Start();`) runs only after that work is done.
- If you call `WriteThreadId()` (with 100 iterations + sleep) **before** starting the other threads, you will see the main thread’s ID printed 100 times first, then the other threads. If you **start** the other threads **first** and then call `WriteThreadId()` on the main thread, all three threads run concurrently and output is interleaved.

**From the transcript:**  
> “Threads are not blocking. So if we move these four lines of code above the main thread then things will be different.”

- So: **same method** can be either blocking (when called directly) or non-blocking (when run on another thread via `Start()`).

---

## 6. Thread priority

- **`Thread.Priority`** (e.g. `ThreadPriority.Highest`, `Lowest`, `Normal`) is a **hint** to the OS scheduler. It does **not** guarantee order or real-time behaviour.

  ```csharp
  thread1.Priority = ThreadPriority.Highest;
  thread2.Priority = ThreadPriority.Lowest;
  Thread.CurrentThread.Priority = ThreadPriority.Normal;
  ```

- With **very short** work (e.g. 100 iterations, no sleep), the high-priority thread often finishes first, then normal, then lowest. So priority can **influence** order when the work is quick.
- With **long** work (e.g. 50 ms per iteration), the scheduler must **preempt** threads to give others a turn. Then you again see interleaved output; priority has less visible effect because time slicing and other factors dominate.

**From the transcript:**  
> “The thread scheduler’s algorithm has many factors. The priority is just one of them. The time slicing is another one.”

- Use priority sparingly; overuse can starve other threads or hurt system responsiveness.

---

## 7. Naming threads

- **`Thread.Name`** is optional and used for debugging (e.g. in debugger or logs). The main thread has no default name; you can set it via `Thread.CurrentThread.Name`.

  ```csharp
  thread1.Name = "Thread1";
  thread2.Name = "Thread2";
  Thread.CurrentThread.Name = "Main thread";
  ```

- In the demo, the method was changed to print `Thread.CurrentThread.Name` instead of `ManagedThreadId` so output is easier to read (e.g. "Thread1", "Thread2", "Main thread").

---

## 8. Full example (from Basic+Syntax)

```csharp
void WriteThreadId()
{
    for (int i = 0; i < 100; i++)
    {
        Console.WriteLine(Thread.CurrentThread.Name);
        // Thread.Sleep(50);
    }
}

Thread thread1 = new Thread(WriteThreadId);
Thread thread2 = new Thread(WriteThreadId);

thread1.Name = "Thread1";
thread2.Name = "Thread2";
Thread.CurrentThread.Name = "Main thread";

thread1.Priority = ThreadPriority.Highest;
thread2.Priority = ThreadPriority.Lowest;
Thread.CurrentThread.Priority = ThreadPriority.Normal;

thread1.Start();
thread2.Start();

WriteThreadId();   // main thread also runs the same task

Console.ReadLine();
```

- Two worker threads run `WriteThreadId` with names and priorities set; the main thread also runs `WriteThreadId`. With `Thread.Sleep(50)` uncommented, all three threads interleave; with it commented, priorities can make one thread finish first.

---

## Summary table

| Concept | Role |
|--------|------|
| **`Thread`** | Type representing an OS thread; constructor takes a `ThreadStart` delegate (method to run). |
| **`thread.Start()`** | Starts the thread; non-blocking; scheduler will run the method on that thread. |
| **`Thread.CurrentThread`** | The thread currently executing the code. |
| **`ManagedThreadId`** | CLR ID of the current thread (integer). |
| **`Thread.Name`** | Optional name for debugging. |
| **`Thread.Priority`** | Hint to OS scheduler (Highest, Normal, Lowest, etc.); one factor among many. |
| **`Thread.Sleep(ms)`** | Suspends current thread for ≥ given milliseconds; useful to observe preemption. |
| **Blocking** | Calling a method directly blocks the current thread until it returns. |
| **Non-blocking** | `Start()` returns immediately; the assigned method runs on another thread. |

---

*Same structure as other professional notes in this section; one file per transcript/topic.*
