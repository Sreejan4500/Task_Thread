# Notes: Monitor (Professional)

*From `4. Monitor.txt` + `4. Use Monitor to add timeout for locks.txt`.*

---

## 1. Positioning `Monitor`

- `Monitor` is another synchronization mechanism for protecting a **critical section**.
- Conceptually, it "monitors" access to that section.
- Functionally, it provides an **exclusive lock**: once one thread enters successfully, other threads targeting the same lock object must wait.

Basic structure:

```csharp
Monitor.Enter(lockObject);
try
{
    // critical section
}
finally
{
    Monitor.Exit(lockObject);
}
```

---

## 2. Relationship to `lock`

One of the key takeaways from the lesson:

- the C# `lock` statement is effectively a simplified form built on `Monitor`

So:

```csharp
lock (counterLock)
{
    counter = counter + 1;
}
```

is conceptually compiled into code very similar to:

```csharp
Monitor.Enter(counterLock);
try
{
    counter = counter + 1;
}
finally
{
    Monitor.Exit(counterLock);
}
```

That is why `lock` has the same basic semantics but a friendlier syntax.

---

## 3. Counter example using `Monitor`

The lesson replaces the prior `lock` example with manual monitor usage:

```csharp
void IncrementCounter()
{
    for (int i = 0; i < 100000; i++)
    {
        Monitor.Enter(counterLock);
        try
        {
            counter = counter + 1;
        }
        finally
        {
            Monitor.Exit(counterLock);
        }
    }
}
```

This solves the same race condition as the `lock` version and produces the expected `200,000`.

---

## 4. Why `try/finally` is mandatory here

With manual `Monitor` usage:

- acquisition and release are explicit,
- so failure to release the lock becomes the developer's responsibility.

The `finally` block ensures:

- the lock is released even if an exception occurs inside the critical section,
- other threads are not permanently blocked waiting for a lock that never exits.

This is precisely the exception-safety behavior that `lock` hides for you.

---

## 5. Where `Monitor` adds value over `lock`

The lesson's main justification for using `Monitor` directly is **additional control**, especially timeout-based acquisition.

The key API shown is:

```csharp
Monitor.TryEnter(ticketsLock, 2000)
```

This attempts to acquire the lock and waits up to `2000` milliseconds.

Return behavior:

- `true` if the lock was acquired
- `false` if the timeout elapsed first

That opens up behaviors that the simple `lock` statement does not express directly.

---

## 6. Ticket-booking example

The second file applies `Monitor.TryEnter(...)` to a simulated booking system:

- requests are queued
- a monitoring thread dequeues work
- worker threads process booking/cancel operations
- ticket availability is a shared resource

Core pattern:

```csharp
if (Monitor.TryEnter(ticketsLock, 2000))
{
    try
    {
        // process booking/cancel
    }
    finally
    {
        Monitor.Exit(ticketsLock);
    }
}
else
{
    Console.WriteLine("The system is busy. Please wait.");
}
```

This allows the application to notify the user when the protected section is congested instead of blocking indefinitely with no feedback.

---

## 7. Why timeout matters in server-style scenarios

The transcript frames this as a user-experience improvement:

- if the protected section is slow or heavily contended,
- threads may wait a long time for the lock,
- silently blocking can feel unfriendly in interactive systems.

Using timeout-based acquisition allows the program to:

- fail fast,
- notify the user,
- optionally retry later.

- **Own note:**  
  This is an early example of a broader systems principle: bounded waiting is often preferable to indefinite blocking in user-facing or service-oriented workloads.

---

## 8. Design implication: keep critical sections short

The booking demo intentionally keeps processing inside the protected section to make timeout behavior visible.

That also highlights an important performance rule:

- the longer a thread holds a lock,
- the higher the contention,
- the more likely other threads are to block or time out.

In production code, critical sections should usually be kept as small as correctness allows.

---

## 9. Practical rule of thumb

- Use `lock` when you only need straightforward in-process exclusive protection.
- Use `Monitor` directly when you need lower-level control, such as timeout-based acquisition logic.

Both mechanisms create exclusive access, but `Monitor` exposes the control points more explicitly.

---

## Summary table

| Concept | Role |
|--------|------|
| **`Monitor.Enter`** | Acquire exclusive access to the critical section. |
| **`Monitor.Exit`** | Release the lock. |
| **`Monitor.TryEnter`** | Attempt acquisition with timeout support. |
| **`try/finally`** | Ensures release even when exceptions occur. |
| **`lock` vs `Monitor`** | `lock` is simpler; `Monitor` is more explicit and flexible. |
| **Main benefit shown** | Timeout logic enables user feedback instead of unbounded blocking. |

---

*This note combines the core monitor explanation and the timeout example into one topic summary.*
