# Notes: AutoResetEvent (Professional)

*From `8. Use AutoResetEvent for signaling.txt` + `AutoResetEvent.txt`.*

---

## 1. Purpose: signaling, not critical-section protection

- **AutoResetEvent** is a synchronization primitive used for **signaling between threads**, not for protecting a critical section.
- It provides a **binary signal** (signaled vs non-signaled). One or more consumer threads wait on the event; a producer thread sets it to release exactly **one** waiting thread per signal.

The lesson frames this with a producer–consumer (or farmer–pigs) analogy: when the producer signals “product ready,” one consumer proceeds; as soon as that consumer proceeds, the signal is **automatically** reset to non-signaled, so only one consumer is released per `Set()`.

---

## 2. API and initial state

Construction:

```csharp
using AutoResetEvent autoResetEvent = new AutoResetEvent(false);
```

- The constructor parameter is the **initial state**: `false` = non-signaled (typical when no product is ready); `true` = signaled at startup.
- The type holds unmanaged resources; the lesson recommends `using` so the instance is disposed when the program exits.

---

## 3. Consumer: WaitOne()

Waiting threads block on:

```csharp
autoResetEvent.WaitOne();
```

- If the event is non-signaled, the thread blocks until another thread calls `Set()`.
- When the event is signaled, **one** waiting thread is released and the event is **automatically** reset to non-signaled. Other waiting threads remain blocked until the next `Set()`.

So at most one waiter proceeds per signal; no explicit reset call is required.

---

## 4. Producer: Set()

The thread that wants to release a waiter calls:

```csharp
autoResetEvent.Set();
```

This sets the event to signaled. One thread that is blocked on `WaitOne()` will be released; the event then returns to non-signaled automatically.

---

## 5. Example: main thread as signal source

The demo uses the main thread to read console input; when the user types “go,” it calls `Set()`. Worker thread(s) run in a loop: `WaitOne()` → do work (e.g. `Thread.Sleep(2000)`) → repeat. So the main thread acts as a “traffic cop” controlling when workers proceed.

---

## 6. Multiple consumer threads

With several worker threads all calling `WaitOne()`:

- Each `Set()` releases **exactly one** waiting thread; which one is not defined.
- After that thread proceeds, the event is auto-reset, so remaining waiters stay blocked until further `Set()` calls.

If the producer signals more often than consumers can process, “signals” can be effectively lost (only one consumer proceeds per signal). The lesson notes that to avoid losing work you typically pair signaling with a queue and proper locking so produced items are stored and consumed under controlled access.

---

## 7. Distinction from locking

The lesson stresses: AutoResetEvent is for **thread interaction and signaling**, not for protecting shared data. It coordinates “when to proceed” rather than “who can access a shared resource.” In practice the signal often indicates that shared data (e.g. a queue) has been updated, but the queue itself must be protected with an appropriate lock or thread-safe collection.

---

## Summary table

| Concept | Role |
|--------|------|
| **AutoResetEvent** | Binary event for one-to-one signaling; one waiter released per `Set()`. |
| **Initial state** | Constructor argument: signaled (`true`) or non-signaled (`false`) at creation. |
| **WaitOne()** | Block until signaled; upon release, event auto-resets to non-signaled. |
| **Set()** | Set event to signaled; exactly one waiting thread is released, then event resets. |
| **Use case** | Producer–consumer or controller–worker coordination, not critical-section protection. |

---

*Professional summary of the AutoResetEvent video and code.*
