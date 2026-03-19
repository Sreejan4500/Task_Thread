# Notes: ManualResetEvent (Professional)

*From `9. Use ManualResetEvent to manually release multiple threads.txt` + `ManualResetEvent.txt`.*

---

## 1. Relation to AutoResetEvent

- **ManualResetEvent** is also a binary (signaled / non-signaled) event primitive.
- The difference is **reset behavior**: the event is **not** automatically reset when a waiting thread is released. The developer must call **Reset()** explicitly to transition the event back to non-signaled.

Terminology in the lesson:

- **Set** = turn the signal on (signaled).
- **Reset** = turn the signal off (non-signaled). With ManualResetEvent, reset is manual; with AutoResetEvent, it happens automatically when one waiter proceeds.

---

## 2. When to use ManualResetEvent

Use it when **one signal should release all currently waiting threads** (or allow multiple threads to pass through until you reset).

Examples from the lesson:

- **Batch availability:** e.g. multiple “pigs” can enter the feeding station at once while the sign is out; the sign is taken down only when the producer explicitly resets.
- **Traffic light:** one green signal allows many vehicles to proceed; the light is turned red (reset) separately.
- **Parallel work on one unit:** e.g. a large file is ready; signal multiple worker threads to start processing different parts; later, signal completion or reset when starting the next batch.

---

## 3. API

The lesson uses the slim variant:

```csharp
using ManualResetEventSlim manualResetEvent = new ManualResetEventSlim(false);
```

- **Initial state:** `false` = non-signaled (waiters block); `true` = signaled (waiters can proceed until reset).
- **Set()** — set the event to signaled. All threads currently waiting on `Wait()` (or that call `Wait()` before the next `Reset()`) can proceed.
- **Reset()** — set the event to non-signaled. Call when you want future waiters to block again.
- **Wait()** — block until the event is signaled. Unlike AutoResetEvent, the event remains signaled after waiters are released until `Reset()` is called.

---

## 4. Example: release multiple threads with one signal

Demo code:

```csharp
using ManualResetEventSlim manualResetEvent = new ManualResetEventSlim(false);

Console.WriteLine("Press enter to release all threads...");

for (int i = 1; i <= 3; i++)
{
    Thread thread = new Thread(Work);
    thread.Name = $"Thread {i}";
    thread.Start();
}

Console.ReadLine();
manualResetEvent.Set();
Console.ReadLine();

void Work()
{
    Console.WriteLine($"{Thread.CurrentThread.Name} is waiting for the signal...");
    manualResetEvent.Wait();
    Thread.Sleep(1000);
    Console.WriteLine($"{Thread.CurrentThread.Name} has been released.");
}
```

All three threads block on `Wait()` until the user presses Enter. One `Set()` releases **all** of them, because the event stays signaled until `Reset()` is called. The example does not call `Reset()`; it only illustrates the “release many at once” behavior.

---

## 5. Why AutoResetEvent releases only one thread

With **AutoResetEvent**, as soon as one waiting thread is released, the runtime resets the event to non-signaled. Other waiters never observe the signaled state and remain blocked. With **ManualResetEvent**, the event remains signaled until an explicit **Reset()**, so every thread that reaches `Wait()` while it is signaled can proceed. That is the essential behavioral difference.

---

## Summary table

| Concept | Role |
|--------|------|
| **ManualResetEvent(Slim)** | Binary event; reset is **manual** via `Reset()`. |
| **Set()** | Set to signaled; all current (and future, until reset) waiters can proceed. |
| **Reset()** | Set to non-signaled; required to block future waiters. |
| **Wait()** | Block until signaled; event remains signaled after waiters are released. |
| **Use case** | One signal to release multiple threads; batch or “go together” scenarios. |

---

*Professional summary of the ManualResetEvent video and code.*
