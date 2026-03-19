# Notes: Nested Locks and Deadlocks (Professional)

*From `12. Nested Locks and Deadlocks.txt` + `Deadlocks.txt`.*

---

## 1. Deadlock

A **deadlock** is a situation where two or more threads are **blocked**, each waiting for a resource or lock held by another. None can make progress, so execution stalls (often indefinitely).

In the simplest form: Thread A holds lock 1 and waits for lock 2; Thread B holds lock 2 and waits for lock 1. Neither will release until it acquires the other lock, so both wait forever.

---

## 2. Nested locks and lock ordering

Deadlocks often arise when threads use **nested locks**: one thread holds lock A and then attempts to acquire lock B, while another holds B and attempts to acquire A. The order in which locks are taken differs between threads, so they can end up holding different locks and waiting for each other.

A standard way to **avoid** this is to enforce a **global lock ordering**: all code that needs multiple locks acquires them in the same order (e.g. always A then B). Then no thread can hold B and wait for A while another holds A and waits for B, so this particular deadlock pattern cannot occur. (Other deadlock patterns are still possible if design is complex.)

---

## 3. E-commerce example in the lesson

The lesson uses an e-commerce-style example with **users** and **orders**:

- One code path **manages users** and may need to touch an order (user → order).
- Another **manages orders** and may need to touch a user (order → user).

So we have two logical resources and two locks: `userLock` and `orderLock`.

- **ManageUser:** acquires `userLock`, then (nested) acquires `orderLock`.
- **ManageOrder:** acquires `orderLock`, then (nested) acquires `userLock`.

The lock order is **opposite** in the two methods. That is enough to allow a deadlock.

---

## 4. Code that deadlocks

```csharp
object userLock = new object();
object orderLock = new object();

Thread thread = new Thread(ManageOrder);
thread.Start();

ManageUser();
thread.Join();

Console.WriteLine("Program finished. Press any key to exit.");
Console.ReadLine();

void ManageUser()
{
    lock (userLock)
    {
        Console.WriteLine("User Management acquired the user lock.");
        Thread.Sleep(2000);

        lock (orderLock)
        {
            Console.WriteLine("User Management acquired the order lock.");
        }
    }
}

void ManageOrder()
{
    lock (orderLock)
    {
        Console.WriteLine("Order Management acquired the order lock.");
        Thread.Sleep(1000);

        lock (userLock)
        {
            Console.WriteLine("Order Management acquired the user lock.");
        }
    }
}
```

- **Main thread** runs `ManageUser`: acquires `userLock`, sleeps 2 seconds, then tries to acquire `orderLock`.
- **Worker thread** runs `ManageOrder`: acquires `orderLock`, sleeps 1 second, then tries to acquire `userLock`.

After about one second: main holds `userLock` and blocks on `orderLock`; worker holds `orderLock` and blocks on `userLock`. Neither can proceed. The program never prints “Program finished.”

---

## 5. Avoiding this deadlock: consistent lock order

To prevent this pattern, use a **single** lock order everywhere. For example, always acquire `userLock` before `orderLock`. Then:

- In **ManageOrder**, even though the logical operation is “manage order,” you would still take `userLock` first, then `orderLock`, so the order matches **ManageUser**.
- No thread can then hold `orderLock` while waiting for `userLock`, because any thread that holds `orderLock` must have already taken `userLock` first. So the circular wait cannot form.

In general: when multiple locks are needed, define a total order and always acquire in that order (and release in reverse order, or at least release as soon as logically possible).

---

## 6. Nested locks as a risk

Nested locking (holding one lock while acquiring another) is a common source of deadlocks. Design choices that reduce the need for nested locks (e.g. coarser locking, lock-free structures, or restructuring so that each path only needs one lock) can simplify reasoning and reduce deadlock risk. When nested locks are necessary, a strict lock ordering discipline is essential.

---

## Summary table

| Concept | Role |
|--------|------|
| **Deadlock** | Two or more threads blocked, each waiting for a resource held by another. |
| **Nested locks** | Holding one lock while acquiring another; increases deadlock risk. |
| **Lock ordering** | Always acquire locks in the same global order to avoid circular wait. |
| **Example** | ManageUser: userLock → orderLock; ManageOrder: orderLock → userLock → deadlock. |
| **Mitigation** | Use a single lock order (e.g. always userLock then orderLock) in all code paths. |

---

*Professional summary of the Nested Locks and Deadlocks video and Deadlocks code.*
