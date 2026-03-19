# Notes: Semaphore (Professional)

*From `7. Semaphore.txt` + `7. Use Semaphore to limit number of threads.txt`.*

---

## 1. What semaphore is for

- A **semaphore** is a synchronization primitive used to control **how many concurrent executions** may enter a protected region.
- Unlike an exclusive lock, which typically permits only one entrant, a semaphore permits up to a configurable number of concurrent entrants.

The lesson emphasizes that semaphore is often used less for protecting a critical section directly and more for **limiting concurrency**.

---

## 2. Why limiting concurrency matters

The transcript revisits the web-server simulation:

- requests are enqueued,
- a monitoring thread watches the queue,
- each request spawns a worker thread.

Without a concurrency limit, a sudden burst of requests can create a very large number of worker threads.

That introduces potential problems:

- excessive thread creation,
- scheduling overhead,
- memory pressure,
- server overload.

This is why real systems often rely on bounded pools and controlled concurrency.

---

## 3. `SemaphoreSlim`

The lesson uses the lighter-weight in-process primitive:

```csharp
using SemaphoreSlim semaphore = new SemaphoreSlim(initialCount: 3, maxCount: 3);
```

Meaning:

- up to 3 concurrent entrants are allowed
- both the starting count and the maximum count are 3

In this example, the semaphore acts as a bounded-capacity gate for request processing.

---

## 4. Basic semantics: `Wait()` and `Release()`

Acquisition:

```csharp
semaphore.Wait();
```

- if the current count is greater than zero, the caller enters immediately and the count is decremented
- if the count is zero, the caller blocks until some other execution releases the semaphore

Release:

```csharp
semaphore.Release();
```

- increments the count
- allows a waiting execution to proceed

So the semaphore effectively acts as a counter-backed admission control mechanism.

---

## 5. Where it is applied in the example

The code performs:

```csharp
semaphore.Wait();
Thread processingThread = new Thread(() => ProcessInput(input));
processingThread.Start();
```

inside the queue-monitoring loop.

This means the system only creates a new active processing path when the semaphore admits it.

Then, in the worker:

```csharp
try
{
    Thread.Sleep(2000);
    Console.WriteLine($"Processed input: {input}");
}
finally
{
    var prevCount = semaphore.Release();
    Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId} released the semaphore. Previous count is: {prevCount}");
}
```

the semaphore is released after processing completes.

---

## 6. Distinctive property: no thread affinity

The lesson explicitly contrasts semaphore with:

- `lock`
- `Monitor`
- `Mutex`
- reader/writer locks

Those constructs are presented as having thread-affinity-like release rules in the teaching context: the same logical owner that enters is responsible for release.

Semaphore is different:

- one thread can call `Wait()`
- another thread can later call `Release()`

That is exactly what happens in the example:

- the monitoring thread acquires admission
- the worker thread releases admission

This flexibility is one of semaphore's defining traits.

---

## 7. Semaphore is not the same as data protection

An important clarification in the lesson:

- limiting concurrency does **not** automatically make every shared data structure safe

The request queue itself is still a shared resource:

```csharp
Queue<string?> requestQueue = new Queue<string?>();
```

So the enqueue and dequeue operations are additionally protected with a normal `lock`:

```csharp
lock (queueLock)
{
    requestQueue.Enqueue(input);
}
```

and:

```csharp
lock (queueLock)
{
    input = requestQueue.Dequeue();
}
```

This is a crucial distinction:

- semaphore limits how many workers proceed
- lock protects queue correctness

They solve different problems.

---

## 8. Why this tradeoff is valuable

Using a semaphore may reduce peak parallelism, but that is intentional.

The goal is not "maximum thread count"; the goal is **sustainable throughput without overload**.

The transcript explicitly compares this to real-world server and database connection limits.

- **Own note:**  
  In practice, bounded concurrency is often essential for system stability. Unbounded concurrency can degrade total throughput rather than improve it.

---

## 9. Local vs global semaphore

The lesson also notes:

- `SemaphoreSlim` is the preferred lighter option for local, in-process use
- a named `Semaphore` can be used when coordination must cross process boundaries

So semaphore follows the same broad pattern seen earlier with mutex:

- local primitives are cheaper
- system-wide primitives are broader in scope but heavier

---

## 10. Key takeaway

Semaphore is best understood as a **concurrency limiter**, not just another lock.

Use it when the system must bound the number of simultaneously active operations, especially in request-processing, worker-pool, or rate-limiting scenarios.

---

## Summary table

| Concept | Role |
|--------|------|
| **Semaphore** | Limits how many concurrent executions can proceed. |
| **`SemaphoreSlim`** | Lightweight in-process semaphore used in the sample. |
| **`Wait()`** | Request admission; blocks when no slots remain. |
| **`Release()`** | Return a slot and increase the count. |
| **Primary use here** | Prevent unbounded worker-thread creation. |
| **Important distinction** | Limiting concurrency does not replace locking for shared data structures. |

---

*This note combines the conceptual explanation and the web-server concurrency-limiting example into one topic summary.*
