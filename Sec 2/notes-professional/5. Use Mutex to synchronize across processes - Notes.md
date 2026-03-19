# Notes: Mutex (Professional)

*From `5. Mutex.txt` + `5. Use Mutex to synchronize across processes.txt`.*

---

## 1. What a mutex is

- A **mutex** is an exclusive synchronization primitive.
- Its usage pattern is similar to `Monitor`, but its most important distinguishing capability is that it can coordinate access **across processes**, not just across threads within one process.

Typical pattern:

```csharp
mutex.WaitOne();
try
{
    // critical section
}
finally
{
    mutex.ReleaseMutex();
}
```

---

## 2. Why mutex exists when `lock` and `Monitor` already exist

The lesson introduces the hierarchy:

- application
- process
- thread

An application may contain multiple processes, and each process may contain one or more threads.

If the shared resource is truly cross-process, in-process primitives are insufficient:

- `lock` and `Monitor` only coordinate threads that share the same process memory space
- separate processes do not share those same synchronization objects automatically

That is why a different primitive is needed.

---

## 3. Cross-process shared resource example

The lesson switches from an in-memory counter to a file-backed counter:

```csharp
string filePath = "counter.txt";
```

Each process repeatedly performs:

1. read the current counter from the file
2. increment it
3. write the updated value back

The file is the new shared resource.

This reproduces the same logical race condition as the earlier thread example, but now the contention occurs between separate running processes.

---

## 4. Why plain `lock` fails here

Even if each process uses code that looks like:

```csharp
lock (fileLockObject)
{
    // read file, increment, write file
}
```

the protection is not global:

- process A has its own in-memory lock object
- process B has a different in-memory lock object

So the two processes are not actually synchronizing with each other.

The transcript demonstrates this experimentally: the final file count remains incorrect when multiple instances run concurrently.

---

## 5. Named mutex

To make a mutex usable across processes, it is given a name:

```csharp
using (var mutex = new Mutex(false, $"GlobalFileMutex:{filePath}"))
```

Important pieces:

- `false`: the creating thread does not take initial ownership
- named mutex: the name identifies the same OS-level synchronization object across processes

As long as different processes use the same mutex name, they contend for the same system-wide mutex.

---

## 6. Example code pattern

The companion code uses:

```csharp
using (var mutex = new Mutex(false, $"GlobalFileMutex:{filePath}"))
{
    for (int i = 0; i < 10000; i++)
    {
        mutex.WaitOne();
        try
        {
            int counter = ReadCounter(filePath);
            counter++;
            WriteCounter(filePath, counter);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }
}
```

That makes the entire read-modify-write sequence exclusive across all participating processes.

---

## 7. Observed outcome

Without synchronization:

- one process doing `10,000` iterations produces `10,000`
- two concurrent processes should produce `20,000`, but the result is lower and inconsistent
- three concurrent processes should produce `30,000`, but again the result is lower without protection

With the named mutex:

- two processes reliably produce `20,000`
- three processes reliably produce `30,000`

This verifies that the mutex is correctly protecting the file-based critical section across processes.

---

## 8. Why `using` matters

The transcript emphasizes the `using` block:

```csharp
using (var mutex = new Mutex(...))
```

because the mutex is an OS-level resource and should be disposed properly.

- **Own note:**  
  This is conceptually similar to disposing streams, file handles, or wait handles. System resources should not be left to linger unnecessarily.

---

## 9. Performance guidance

The lesson gives a practical rule:

- prefer `lock` or `Monitor` for synchronization within a single process
- use `Mutex` only when cross-process coordination is required

Reason:

- mutexes are heavier,
- they rely on operating system resources,
- they carry more overhead than in-process locking primitives.

So mutex is more powerful in scope, but more expensive.

---

## 10. Key takeaway

- `Mutex` is the right choice when the shared resource is visible to multiple processes, such as a file.
- It is not the first choice for normal intra-process thread synchronization.

This lesson is therefore the bridge from thread-only synchronization to OS-visible synchronization.

---

## Summary table

| Concept | Role |
|--------|------|
| **Mutex** | Exclusive synchronization primitive that can coordinate across processes. |
| **`WaitOne()`** | Acquire ownership before entering the critical section. |
| **`ReleaseMutex()`** | Release ownership after the protected work finishes. |
| **Named mutex** | OS-visible mutex shared across multiple processes. |
| **Primary use case** | Protecting shared resources like files across process boundaries. |
| **Guidance** | Prefer `lock`/`Monitor` unless cross-process synchronization is required. |

---

*This note combines the conceptual explanation and the file-based counter example into one topic summary.*
