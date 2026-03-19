# Notes: Reader and Writer Locks (Professional)

*From `6. Reader and Writer Locks.txt` + `6. ReaderWriterLock.txt`.*

---

## 1. Why a plain exclusive lock can be suboptimal

- `lock`, `Monitor`, and `Mutex` are all fundamentally **exclusive** mechanisms.
- Once acquired, they block all other threads from entering the protected section.

That is correct but sometimes overly restrictive.

In read-heavy workloads:

- many threads may only need to inspect shared state,
- concurrent reads do not inherently mutate that state,
- forcing readers to serialize can reduce throughput unnecessarily.

This is the motivation for a reader/writer lock.

---

## 2. Reader/writer lock semantics

A reader/writer lock behaves as follows:

- multiple **readers** may hold the lock concurrently
- a **writer** requires exclusive access
- while any reader holds the lock, writers must wait
- while a writer holds the lock, all other readers and writers must wait

So the lock is:

- shared for readers
- exclusive for writers

This balances correctness with improved performance in read-dominant scenarios.

---

## 3. Real-world examples from the lesson

The transcript uses two practical examples:

### Database systems

- `SELECT` operations behave like readers
- `INSERT`, `UPDATE`, and `DELETE` behave like writers
- the discussion maps naturally to database shared and exclusive lock concepts

### Web server shared cache

- application configuration is loaded into an in-memory cache
- many requests may read that cache
- some requests may update it

This pattern is ideal for reader/writer synchronization: frequent reads, occasional writes.

---

## 4. Why a regular `Dictionary` needs protection

The sample cache is backed by:

```csharp
private Dictionary<int, string> _cache = new Dictionary<int, string>();
```

with methods for add/update and retrieval.

The lesson stresses that a normal `Dictionary<TKey, TValue>` is not inherently safe for mixed concurrent access involving writes.

Why:

- operations are not guaranteed to be atomic in the concurrency sense,
- internal state transitions can interleave,
- concurrent access can lead to inconsistent internal state or intermittent failures.

---

## 5. `ReaderWriterLockSlim`

The implementation uses:

```csharp
private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
```

This is the slimmer modern .NET reader/writer lock implementation.

The two relevant APIs shown are:

- `EnterWriteLock()` / `ExitWriteLock()`
- `EnterReadLock()` / `ExitReadLock()`

---

## 6. Write path

The add/update path uses a write lock:

```csharp
public void Add(int key, string value)
{
    bool lockAcquired = false;
    try
    {
        _lock.EnterWriteLock();
        lockAcquired = true;
        _cache[key] = value;
    }
    finally
    {
        if (lockAcquired) _lock.ExitWriteLock();
    }
}
```

Why write lock:

- the operation mutates shared state,
- mutation requires exclusivity to preserve cache integrity.

---

## 7. Read path

The retrieval path uses a read lock:

```csharp
public string? Get(int key)
{
    bool lockAcquired = false;
    try
    {
        _lock.EnterReadLock();
        lockAcquired = true;
        return _cache.TryGetValue(key, out var value) ? value : null;
    }
    finally
    {
        if (lockAcquired) _lock.ExitReadLock();
    }
}
```

Why read lock:

- multiple callers can safely inspect the cache at once,
- as long as there is no concurrent writer.

This is the main performance gain over a plain exclusive lock.

---

## 8. Why the `lockAcquired` flag is present

The transcript deliberately guards the `Exit...Lock()` calls with:

```csharp
bool lockAcquired = false;
```

followed by:

```csharp
if (lockAcquired) _lock.ExitReadLock();
```

or:

```csharp
if (lockAcquired) _lock.ExitWriteLock();
```

Reason:

- if acquisition fails by throwing before the lock is actually obtained,
- blindly calling `Exit...Lock()` would be incorrect.

This pattern ensures release only happens after successful acquisition.

---

## 9. When to prefer this approach

Reader/writer locks are most appropriate when:

- reads are frequent,
- writes are relatively infrequent,
- shared data must remain consistent,
- reader concurrency materially helps throughput.

If writes dominate or lock hold times are large, the benefit may shrink.

- **Own note:**  
  This is why reader/writer locking is common in caches, configuration stores, routing tables, and metadata registries, but not automatically the best choice for every shared object.

---

## 10. Relation to concurrent collections

The transcript also previews another design direction:

- use built-in concurrent data structures such as `ConcurrentDictionary`

That shifts some concurrency management into the collection itself.

So the two broad options are:

- protect a normal collection with explicit synchronization
- use a collection designed for concurrent access

This lesson focuses on the first approach.

---

## Summary table

| Concept | Role |
|--------|------|
| **Reader/writer lock** | Allows concurrent reads while preserving exclusive writes. |
| **Read lock** | Shared lock for read-only access. |
| **Write lock** | Exclusive lock for mutation. |
| **`ReaderWriterLockSlim`** | .NET type used for the sample implementation. |
| **Best fit** | Read-heavy workloads such as caches or configuration stores. |
| **Alternative** | Concurrent collections can sometimes replace manual locking. |

---

*This note combines the conceptual explanation and the cache implementation into one topic summary.*
