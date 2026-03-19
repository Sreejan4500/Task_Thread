# Notes: Exclusive Locks (Professional)

*From `3. Exclusive Locks.txt` + `3. Exclusive+Lock.txt`.*

---

## 1. What an exclusive lock does

- An **exclusive lock** allows at most **one thread** to execute the protected section at any given time.
- All other threads attempting to enter the same protected section must wait until the current holder exits.
- In C#, the standard syntax is the `lock` statement:

```csharp
lock (counterLock)
{
    counter = counter + 1;
}
```

The lock body normally contains the critical section.

---

## 2. Lock object

The example declares a dedicated lock object:

```csharp
object counterLock = new object();
```

and then uses it around the shared increment:

```csharp
lock (counterLock)
{
    counter = counter + 1;
}
```

Key practical point from the lesson:

- use a dedicated object whose only purpose is to protect that critical section

- **Own note:**  
  This avoids accidental coupling with unrelated code that might otherwise lock on the same instance.

---

## 3. Counter example with lock

The companion file shows the complete pattern:

```csharp
int counter = 0;
object counterLock = new object();

Thread thread1 = new Thread(IncrementCounter);
Thread thread2 = new Thread(IncrementCounter);

thread1.Start();
thread2.Start();

thread1.Join();
thread2.Join();

Console.WriteLine($"Final counter value is: {counter}");

void IncrementCounter()
{
    for (int i = 0; i < 100000; i++)
    {
        lock (counterLock)
        {
            counter = counter + 1;
        }
    }
}
```

This protects the read-modify-write sequence so only one thread can perform it at a time.

---

## 4. Why this solves the race condition

The increment operation is still not inherently atomic:

```csharp
counter = counter + 1;
```

However, the lock makes the critical section **effectively atomic with respect to other threads** because:

- only one thread can enter the protected block,
- no competing thread can read or write `counter` inside that same block concurrently,
- the lost-update interleaving is therefore prevented.

The transcript explicitly frames this as making the code "behave like" an atomic operation.

---

## 5. Observed result

- With no synchronization, the final count is typically less than `200,000`.
- With the `lock` added, the final count becomes consistent.

Given the code:

- 2 threads
- 100,000 increments each

the correct final result is:

- `200,000`

- **Own note:**  
  The transcript briefly says `300,000` once, but that is a verbal slip; the code shown clearly implies an expected result of `200,000`.

---

## 6. Exception safety

The lesson points out that `lock` includes built-in exception-safety behavior.

That means:

- if code inside the lock throws,
- the runtime still ensures the lock is released appropriately.

This is one of the reasons `lock` is usually preferred over lower-level manual constructs when simple exclusivity is sufficient.

---

## 7. Why `lock` is often preferred

Compared with manual synchronization APIs, `lock` is:

- concise,
- readable,
- harder to misuse,
- automatically structured around acquisition and release.

For straightforward in-process synchronization of a critical section, it is typically the most ergonomic choice.

---

## 8. Version note from the transcript

The lesson mentions that:

- in current/pre-C# 13 style, using a dedicated `object` is the common approach
- newer language/runtime versions introduce more specialized lock-related support

The conceptual guidance remains the same:

- choose one dedicated synchronization object,
- use it consistently to protect the shared state.

---

## 9. Scope limitation

- A normal `lock` only coordinates execution **inside the same process**.
- It does not solve cross-process synchronization.

That limitation motivates the later lesson on `Mutex`.

---

## Summary table

| Concept | Role |
|--------|------|
| **Exclusive lock** | Ensures only one thread can execute the protected block at a time. |
| **`lock` statement** | Idiomatic C# syntax for in-process exclusive synchronization. |
| **Lock object** | Shared synchronization token used by all participating threads. |
| **Effect** | Prevents interleaving inside the critical section. |
| **Benefit** | Produces correct, repeatable results for shared-state updates. |
| **Limitation** | Applies within a process, not across processes. |

---

*This note combines the conceptual explanation and the counter example into one topic summary.*
