# Notes: Critical Section and Atomic Operation (Professional)

*From `2. Critical Section and Atomic Operation.txt`.*

---

## 1. Critical section

- A **critical section** is the part of the code that accesses a **shared resource**.
- In the running example, the shared resource is the externally declared `counter` variable.
- By contrast, the loop variable `i` is local to each thread and therefore not shared.

The critical statement is:

```csharp
counter = counter + 1;
```

It is critical because multiple threads may execute it against the same shared state.

---

## 2. Atomic operation

- An **atomic operation** is an operation that is **indivisible**.
- It executes as one unit from the perspective of other threads.
- No other thread can observe or interfere with an intermediate state in the middle of the operation.

The lesson uses this concept to explain why some shared-resource operations are safe and others are not.

---

## 3. Why the increment is not atomic

Although the source code shows one line:

```csharp
counter = counter + 1;
```

the action is logically separable into multiple steps:

```csharp
var temp = counter;
counter = temp + 1;
```

That means:

1. the current value is read,
2. a new value is computed,
3. the updated value is written back.

Since other threads can interleave between those steps, the operation is not atomic in the sense relevant to synchronization.

---

## 4. Relationship between the two concepts

The lesson emphasizes that the synchronization bug arises from the combination of:

1. a **critical section** exists because shared data is being accessed
2. the operation performed in that critical section is **not atomic**

If the shared-state operation were atomic, two threads would not lose updates in the same way.

If the data were not shared, concurrency would not create the same correctness issue.

So the actual danger is the intersection of:

- shared access
- non-atomic work

---

## 5. Interleaving example

Assume `counter == 1000`.

Then the following interleaving is possible:

1. thread 1 reads `1000`
2. thread 2 reads `1000`
3. thread 1 computes and writes `1001`
4. thread 2 computes and writes `1001`

The expected logical outcome of two increments is `1002`, but the actual result is `1001`.

This is the classic **lost update** problem.

---

## 6. Why these definitions matter

These two concepts provide the vocabulary for nearly all subsequent synchronization techniques:

- identify the **critical section**
- ensure the work inside it behaves safely, often by making access effectively exclusive

The goal of locks, monitors, mutexes, and related constructs is not to alter the original operation itself, but to control when and how threads are allowed to execute it.

---

## 7. Practical design implication

- Shared mutable state should always be treated as suspicious in concurrent code.
- Whenever multiple threads can read and write the same resource, examine whether the operations involved are truly atomic.
- If they are not, introduce synchronization around the critical section.

- **Own note:**  
  In real systems, many operations that look simple at source-code level are not safe under concurrency. The safe assumption is that any read-modify-write sequence is vulnerable unless explicitly synchronized.

---

## Summary table

| Concept | Meaning |
|--------|---------|
| **Critical section** | Code that accesses a shared resource. |
| **Shared resource** | Data reachable from multiple threads, such as `counter`. |
| **Atomic operation** | Indivisible operation that cannot be meaningfully interrupted by other threads. |
| **Non-atomic increment** | A read-modify-write sequence that can interleave with another thread. |
| **Lost update** | One thread's result overwrites another thread's update. |
| **Main lesson** | Synchronization problems require both shared access and non-atomic work. |

---

*This note isolates the two foundational ideas that the rest of the section builds on.*
