# Notes: Basic Syntax to Start a Thread (Kid-Friendly)

*From the same video + code; explained in simple words.*

---

## 1. A task = a method (like one step in a recipe)

**Simple idea:** In a program, we do work by **calling a method** (a function).  
For example, we can make a method that just prints “which worker is doing this right now.”  
That “worker” is the **thread**. Each thread has an ID number (like a name tag).

- **To a kid:**  
  Think of the method as **one step** in a recipe. When you call it from the main program, the **main thread** (the first worker) does that step. So the main thread has an ID (often 1). If we run the same step on a **different** worker (a new thread), that worker has a different ID (like 7 or 8).

---

## 2. How to start a new thread (3 steps)

**From the video:**  
> “You declare the thread, you assign a task to the thread, and then you say thread one dot start.”

**Simple idea:**  
To have **another** worker do a task (not the main one), we use the **`Thread`** class.  
We do 3 things:

1. **Create** a thread and tell it **which method** to run (the “task”).
2. **Start** the thread with **`Start()`** so it actually begins working.

- **To a kid:**  
  - **Create:** “Here is a new worker (thread), and their job is to run this method.”  
  - **Start:** “Go!” — and the worker starts. The main program does **not** wait; it keeps going. So now two workers are doing things at the same time (the main thread and the new one).

**In code:**

```csharp
Thread thread1 = new Thread(WriteThreadId);  // give the thread a task (the method)
thread1.Start();                              // tell it to start
```

---

## 3. Same task, many threads (a small team)

**Simple idea:**  
We can create **two** (or more) threads and give them the **same** method.  
Each thread is a different “worker,” so each has its own ID. When you run the program, you might see one thread print its ID many times, then the other—or they might take turns. We **cannot** know the exact order.

- **To a kid:**  
  It’s like having two friends both doing the same chore (e.g. “write your name 100 times”). They work at the same time. Sometimes one finishes first, sometimes they switch. The **scheduler** (the “manager” from the other notes) decides who gets to work when.

---

## 4. Slowing down with Sleep (so we can see them take turns)

**Simple idea:**  
If the task is super quick (e.g. print 100 times with no pause), one thread might finish all 100 before the other really starts.  
To **see** them take turns, we can **pause** the thread for a short time with **`Thread.Sleep(50)`** (50 = 50 milliseconds). Then the scheduler will switch to the other thread, and we see mixed output (7, 8, 7, 8, 7, 7, 8…).

- **To a kid:**  
  **Sleep** = “rest for a tiny moment.” When one worker rests, the manager can say “okay, your turn” to another worker. So we see both workers’ names (or IDs) mixed together in the output.

---

## 5. Blocking vs not blocking (waiting or not?)

**Simple idea:**  
- If we **call the method directly** (e.g. `WriteThreadId();`) in the main program, the **main thread** does the whole task and **waits** until it’s done. Nothing after that line runs until it finishes. That’s **blocking**.  
- If we **start another thread** with `thread1.Start()`, the main program does **not** wait. It keeps going. That’s **non-blocking**.

- **To a kid:**  
  - **Blocking:** “You do this whole chore; I’ll wait here until you’re done.”  
  - **Non-blocking:** “You do this chore in the other room; I’m going to do something else.” So the **same** method can either “block” the main thread (if we call it directly) or “not block” (if we run it on another thread with `Start()`).

---

## 6. Priority (a hint: “this one is more important”)

**Simple idea:**  
We can give a thread a **priority** (e.g. Highest, Normal, Lowest). That’s like telling the scheduler “this thread is more important” or “this one can wait a bit.”  
It’s only a **hint**. When the task is very short, the high-priority thread often finishes first. When the task is long (e.g. with Sleep), the scheduler still switches between threads a lot, so we see them take turns and priority doesn’t always “win.”

- **To a kid:**  
  Priority is like saying “this worker is more important.” The manager (scheduler) might listen sometimes, but they also want to be fair and give everyone a turn. So we can’t rely on priority to always decide who goes first.

---

## 7. Giving threads names (so we know who is who)

**Simple idea:**  
Instead of printing the thread **ID** (a number), we can give each thread a **name** (e.g. "Thread1", "Main thread"). Then when we print the name, the output is easier to read: "Thread1", "Thread2", "Main thread."

- **To a kid:**  
  Names are like name tags. The method can print “who am I?” — and we can make it show the thread’s **name** instead of a number, so it’s easier to see which worker did what.

---

## 8. A full example (what the code looks like)

We create a method that prints the **current thread’s name** 100 times (and we can add `Thread.Sleep(50)` to see them take turns). We create two threads, give them names and priorities, start them, and also run the same method on the main thread. So three “workers” are doing the same task.

```csharp
void WriteThreadId()
{
    for (int i = 0; i < 100; i++)
    {
        Console.WriteLine(Thread.CurrentThread.Name);
        // Thread.Sleep(50);   // uncomment to see them take turns
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
WriteThreadId();   // main thread does the same task

Console.ReadLine();
```

---

## Quick recap (kid version)

| Thing | Simple idea |
|-------|-------------|
| **Method** | The “task” or step we want a thread to do |
| **Thread** | A worker that can run a method |
| **`new Thread(method)`** | Create a new worker and give them a task |
| **`thread.Start()`** | Tell the worker to start (main program doesn’t wait) |
| **Blocking** | Calling the method directly = main thread waits until it’s done |
| **Non-blocking** | `Start()` = main program keeps going; the thread runs in parallel |
| **Thread.Sleep(50)** | Pause for a bit so we can see threads take turns |
| **Priority** | A hint (“more important” or “can wait”); scheduler may or may not follow it |
| **Thread.Name** | A name for the thread so we can tell who is who in the output |

---

*Same style as the other kid notes in this folder; one file per topic.*
