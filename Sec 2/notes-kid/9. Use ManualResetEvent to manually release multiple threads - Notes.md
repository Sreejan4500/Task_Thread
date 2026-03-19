# Notes: ManualResetEvent — Release Multiple Threads at Once (Kid-Friendly)

*From `9. Use ManualResetEvent to manually release multiple threads.txt` + `ManualResetEvent.txt`, explained simply.*

---

## 1. How is it different from AutoResetEvent?

**ManualResetEvent** is also a binary signal (on/off), but the **reset is not automatic**.

- **AutoResetEvent:** when one waiting thread proceeds, the signal is **automatically** turned off.
- **ManualResetEvent:** the signal stays on until **you** call `Reset()` to turn it off.

So you turn it **on** with `Set()` and turn it **off** with `Reset()` yourself.

- **To a kid:**  
  With the pig feeding station: with AutoResetEvent, the sign disappears as soon as one pig goes in. With ManualResetEvent, the sign stays out until the farmer takes it down. So many pigs can go in while the sign is still out.

---

## 2. When do we need this?

When you want **many threads** to proceed at the same time with one signal:

- **Farmer analogy:** the farmer puts out a **batch** of food. All three pigs can go to the feeding station and eat at the same time. The sign stays out until the farmer takes it down.
- **Traffic light:** when it turns green, **everyone** goes; when it turns red, everyone stops. Not one car at a time.
- **Big file:** one thread produces a file; you signal several worker threads to “go” and process different parts at the same time.

- **To a kid:**  
  Like a starting gun for a race: one “go” and everyone runs at once, instead of letting only one runner go at a time.

---

## 3. Basic syntax

The lesson uses the **slim** version:

```csharp
using ManualResetEventSlim manualResetEvent = new ManualResetEventSlim(false);
```

- `false` = initial state is **off** (threads that wait will block until you call `Set()`).
- To turn the signal **on:** `manualResetEvent.Set();`
- To turn the signal **off:** `manualResetEvent.Reset();` (you do this when you want to make waiters block again).

---

## 4. Consumer: Wait()

Waiting threads call:

```csharp
manualResetEvent.Wait();
```

- If the signal is **off**, they block.
- When you call `Set()`, **all** threads that are waiting at `Wait()` can proceed (unlike AutoResetEvent, where only one proceeds and the signal turns off).

---

## 5. Example from the code

Three worker threads wait; the main thread releases them all with one Enter key:

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
manualResetEvent.Set();   // Release all waiting threads
Console.ReadLine();

void Work()
{
    Console.WriteLine($"{Thread.CurrentThread.Name} is waiting for the signal...");
    manualResetEvent.Wait();
    Thread.Sleep(1000);
    Console.WriteLine($"{Thread.CurrentThread.Name} has been released.");
}
```

- All three threads block on `Wait()` until the user presses Enter.
- One `Set()` lets **all three** pass and run their work.

- **To a kid:**  
  Three workers stand at the door. When you put the sign out once, all three can go in, not just one.

---

## 6. Why only one thread with AutoResetEvent?

With **AutoResetEvent**, as soon as one thread passes the wait, the signal is turned off. So the other waiting threads never see “on” and stay waiting.

With **ManualResetEvent**, the signal stays on until you call `Reset()`, so every thread that was waiting gets a chance to pass through.

---

## Quick recap (kid version)

| Thing | Simple idea |
|-------|-------------|
| **ManualResetEvent** | Binary signal you turn on with `Set()` and off with `Reset()` yourself. |
| **Set()** | Turn the signal on; all waiting threads can proceed. |
| **Reset()** | Turn the signal off (when you want waiters to block again). |
| **Wait()** | Wait until the signal is on; then proceed (signal stays on until Reset). |
| **Difference from AutoResetEvent** | Manual = sign stays out so many can go; Auto = sign goes away after one goes. |
| **Use when** | You want one signal to release **multiple** threads at once. |

---

*Same style as the other kid notes in this folder; uses the ManualResetEvent video and code.*
