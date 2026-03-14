# Notes: Reader and Writer Locks (Kid-Friendly)

*From `6. Reader and Writer Locks.txt` + `6. ReaderWriterLock.txt`, explained simply.*

---

## 1. Why a normal lock is sometimes too strict

A regular `lock`, `Monitor`, or `Mutex` is **exclusive**.

That means:

- one thread gets in,
- everybody else waits.

That is great when threads are **changing** shared data.

But what if many threads only want to **read** the same data?

Reading usually does not change anything.

So forcing all readers to wait one by one can slow the program down.

- **To a kid:**  
  If ten kids only want to look at the class timetable on the wall, they should not need to stand in a one-person line. But if someone wants to erase and rewrite it, then others should wait.

---

## 2. The big idea of reader/writer locks

A **reader/writer lock** allows:

- many readers at the same time
- but only one writer at a time

And when a writer is writing:

- nobody else should read or write that same shared resource at that moment

So:

- read lock = shared among readers
- write lock = exclusive

---

## 3. Real-world examples from the lesson

The teacher gives two good examples:

### Database systems

- many users may run `SELECT` queries at the same time
- reads can often happen together
- updates/inserts/deletes must be exclusive

### Web server shared cache

- a web app may keep configuration data in memory
- many requests may read that cache
- some requests may update it

In both cases, letting all readers move together improves performance.

---

## 4. Why the dictionary cache can be dangerous

The example uses a shared cache like this:

```csharp
private Dictionary<int, string> _cache = new Dictionary<int, string>();
```

and methods like:

```csharp
_cache[key] = value;
return _cache.TryGetValue(key, out var value) ? value : null;
```

The lesson explains that a normal `Dictionary` is **not thread-safe** for mixed concurrent reads/writes.

So if many threads use it at the same time:

- its internal state can become inconsistent,
- weird errors may appear once in a while,
- the bug may be hard to reproduce.

---

## 5. The tool used in code: `ReaderWriterLockSlim`

The lesson uses:

```csharp
private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
```

This is the lighter modern version commonly used in .NET.

Then:

- `EnterWriteLock()` for writing
- `ExitWriteLock()` when done
- `EnterReadLock()` for reading
- `ExitReadLock()` when done

---

## 6. Write side example

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

Why a **write lock**?

Because adding or updating changes the shared data.

So it must be exclusive.

---

## 7. Read side example

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

Why a **read lock**?

Because many readers can safely look at the cache at the same time, as long as no writer is changing it.

---

## 8. Why the `lockAcquired` flag is used

The lesson adds a boolean like:

```csharp
bool lockAcquired = false;
```

Then it sets it to `true` only after the lock is actually entered.

That way, the code only exits the lock if it really got it.

- **To a kid:**  
  Only try to hand back the room key if you actually received the key first.

This is a safety pattern in case an exception happens before lock acquisition finishes.

---

## 9. When should you use this kind of lock?

Reader/writer locks are best when:

- reads happen very often,
- writes happen less often,
- letting readers run together gives better performance.

If almost everything is writing, a simple exclusive lock may be enough.

- **Own note:**  
  Reader/writer locks are most helpful in read-heavy workloads. If write operations are frequent, the extra complexity may not give much benefit.

---

## 10. Related idea: concurrent collections

The lesson also mentions that later there are thread-safe collections such as `ConcurrentDictionary`.

If you use a collection that is already built for concurrency, you may not need to manually wrap a normal dictionary with this lock pattern.

But for a plain `Dictionary`, you do need protection.

---

## Quick recap (kid version)

| Thing | Simple idea |
|------|-------------|
| **Reader/writer lock** | Lets many readers in together, but only one writer. |
| **Read lock** | Shared lock for safe reading. |
| **Write lock** | Exclusive lock for changing data. |
| **Why useful** | Faster than a normal exclusive lock when lots of work is reading. |
| **Example shared data** | A web app cache or database-like shared data. |
| **`ReaderWriterLockSlim`** | The .NET class used in the example. |

---

*This note combines the concept lesson and the cache example into one kid-friendly summary.*
