# Notes: Thread Safety (Professional)

*From `11. Thread Safety.txt`.*

---

## 1. Definition

In multithreaded programming, a **function**, **data structure**, or **class** is **thread safe** when it can be used **concurrently by multiple threads** without introducing:

- race conditions,
- undefined or unexpected behavior,
- data corruption.

Implication: inside the type or method, appropriate synchronization (e.g. locking, lock-free algorithms, or immutability) is applied so that concurrent access does not violate invariants or leave shared state partially updated.

---

## 2. Thread-unsafe usage

When a type is **not** thread safe and multiple threads use it without external synchronization:

- Shared data can be corrupted (e.g. wrong values, lost updates, duplicate or missing queue elements).
- Results depend on scheduling and timing; behavior is non-deterministic and hard to reproduce.
- Bugs may appear only under load or in specific environments.

So either the type must be documented as thread safe and implemented accordingly, or the caller must ensure all concurrent access is properly synchronized.

---

## 3. Example: Queue in producer–consumer

The lesson refers to the producer–consumer example that uses a **Queue&lt;T&gt;**:

- The standard **Queue&lt;T&gt;** is **not** thread safe. Its documentation states that it does not support concurrent access from multiple threads.
- If one thread enqueues while another dequeues (or multiple threads enqueue/dequeue), the internal state can be corrupted and behavior is undefined.
- Therefore the lesson wraps all queue access in a **lock** (or equivalent) so that only one thread at a time modifies the queue. That makes the **usage** of the queue thread safe at the application level.

So “thread safe” can mean: the type itself guarantees safety, or the caller ensures safety by synchronizing all access.

---

## 4. What makes a type or API thread safe

A type or API is thread safe when:

- All access to shared mutable state is coordinated (e.g. locks, lock-free structures, or confinement to a single thread), and
- no sequence of concurrent calls can leave the object in an invalid state or produce incorrect results.

When documentation says “thread safe,” it means the type handles concurrency internally (or is designed so that concurrent use is safe). When it says “not thread safe,” the caller is responsible for serializing or otherwise coordinating access.

---

## 5. Relation to the synchronization section

The synchronization section (lock, Monitor, Mutex, semaphore, reader/writer locks, etc.) is essentially about **achieving** thread safety for shared resources: we use those primitives to protect shared data and make our code safe under concurrent use. So “thread safety” is the property we aim for; the primitives are the mechanisms.

---

## Summary table

| Concept | Role |
|--------|------|
| **Thread safe** | Safe for concurrent use by multiple threads; no races or corruption. |
| **Thread unsafe** | Concurrent use without synchronization can cause corruption or undefined behavior. |
| **Queue&lt;T&gt;** | Not thread safe; wrap access in a lock (or use a thread-safe collection). |
| **Responsibility** | Either the type guarantees thread safety, or the caller must synchronize access. |

---

*Professional summary of the Thread Safety video.*
