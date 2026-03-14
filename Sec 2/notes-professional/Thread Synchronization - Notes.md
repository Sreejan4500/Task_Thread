# Notes: Thread Synchronization (Professional)

*From `1. Thread Synchronization Overview.txt` + `1. Threads+Synchronization+Overview.txt`.*

---

## 1. Why synchronization is needed

- Multithreaded programs become difficult when multiple threads must access **shared resources**.
- Shared access introduces the possibility of **conflicts**, **race conditions**, and **inconsistent results**.
- The lesson frames synchronization as the set of techniques used to coordinate parallel work so shared state remains correct.

The transcript uses two analogies:

- a shared kitchen where multiple cooks compete for the same tools and space
- a shared bank account where two tellers independently authorize withdrawals based on the same stale balance

In both cases, the underlying problem is concurrent access to the same resource without proper coordination.

---

## 2. Bank-account analogy and race condition intuition

- Two account holders, Tom and Jerry, each try to withdraw `$800` from a shared account containing `$1000`.
- Two separate tellers inspect the account balance at approximately the same time.
- Each teller sees a sufficient balance and approves the withdrawal.
- Both withdrawals succeed, producing an overdrawn account.

This illustrates the core synchronization failure:

- both workers act on the same old state,
- neither sees the other worker's update in time,
- the final system state becomes invalid.

That is the exact same pattern later reproduced in code.

---

## 3. Code example: shared counter

The companion code uses a shared integer counter:

```csharp
int counter = 0;

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
        counter = counter + 1;
    }
}
```

- Both threads execute the same `IncrementCounter()` routine.
- Each thread attempts `100,000` increments.
- The expected final value is therefore `200,000`.

---

## 4. Synchronous vs parallel execution

The lesson first shows a serialized variant:

- start thread 1
- `Join()` thread 1
- start thread 2
- `Join()` thread 2

That effectively eliminates overlap and produces the expected `200,000`.

When both threads are started before joining:

```csharp
thread1.Start();
thread2.Start();
thread1.Join();
thread2.Join();
```

- they run concurrently,
- the final result is typically **less than `200,000`**,
- the exact value varies across runs.

This is the first concrete demonstration of a race condition in the section.

---

## 5. Why `counter = counter + 1` is unsafe

The key lesson point is that:

```csharp
counter = counter + 1;
```

is not an indivisible operation.

Conceptually it behaves more like:

```csharp
var temp = counter;
counter = temp + 1;
```

That means:

1. read the current value
2. compute a new value
3. write the new value back

If two threads interleave those steps, one update can overwrite the other.

Example:

- both threads read `1000`
- thread 1 writes `1001`
- thread 2, still holding the old read, also writes `1001`

Two attempted increments result in a single net increment.

---

## 6. Observable symptoms

The transcript explicitly identifies the observed behavior as:

- **race conditions**
- **inconsistent behavior**

This inconsistency appears because scheduling varies from run to run:

- the source code is the same,
- the number of loop iterations is the same,
- but the timing of reads and writes changes.

As a result, the program produces non-deterministic output.

---

## 7. Definition of thread synchronization

- **Thread synchronization** is the technique used to coordinate parallel access to shared resources so that concurrent execution remains correct.
- Its purpose is not to remove concurrency entirely, but to control it where correctness depends on ordered access.

The rest of the section introduces concrete synchronization mechanisms that solve this problem in different scenarios.

---

## 8. Practical takeaway

- Concurrency alone is not the problem.
- The problem begins when concurrent work touches **shared mutable state**.
- Whenever multiple threads read and write the same resource, the default assumption should be that synchronization may be required.

- **Own note:**  
  This example is intentionally minimal, but the same failure mode appears in production systems with bank balances, inventory counts, caches, file updates, and UI state.

---

## Summary table

| Concept | Meaning |
|--------|---------|
| **Shared resource** | Data used by multiple threads, such as `counter`. |
| **Race condition** | Incorrect behavior caused by timing-dependent interleaving of concurrent operations. |
| **Inconsistent behavior** | Different results across runs despite the same source code and logical workload. |
| **`Join()`** | Serializes work by forcing one thread to wait for another. |
| **Thread synchronization** | Techniques that coordinate concurrent access to shared state. |
| **Main lesson** | Parallel access to shared mutable data must be controlled. |

---

*This note combines the transcript explanation and the counter example into one topic summary.*
