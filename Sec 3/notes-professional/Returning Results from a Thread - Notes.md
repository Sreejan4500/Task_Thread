# Notes: Returning Results from a Thread (Professional)

*From `4. Returning results from a thread.txt`.*

---

## 1. No return value from Thread

The **Thread** class does not provide a built-in mechanism to return a value from the thread’s entry point. Unlike **Task&lt;T&gt;** (and related APIs), there is no **Result** or similar property. So any “result” must be communicated by other means.

---

## 2. Shared variable pattern

The standard approach with **Thread** is to use a **shared variable** (or shared structure) that the worker thread writes and the main (or another) thread reads. To avoid reading before the worker has written, the reader must **synchronize** with the worker’s completion—typically by calling **Join()** on the thread (or otherwise waiting for completion) before reading the shared state.

Pattern:

- Declare a variable (e.g. `string? result = null`) in a scope visible to both threads.
- Start the worker thread; the worker computes and assigns to that variable.
- Call **thread.Join()** (or equivalent) so the reader blocks until the worker has finished.
- After Join returns, read the variable. If the worker is the only writer, no further synchronization is needed for that read; otherwise use appropriate memory ordering or locking.

---

## 3. Example and divide-and-conquer

The lesson implements a minimal example: main thread creates a thread, starts it, Join()s, then prints the value the worker wrote to a shared string. The same pattern appears in the **divide-and-conquer** example: multiple threads each write a partial result to their own (or a shared) variable; the main thread Join()s all workers, then aggregates (e.g. sums) those values. So “returning” from a thread is implemented as: write to shared state, synchronize on completion (Join), then read.

---

## 4. Task and results

The lesson notes that when using **Task**, there is built-in support for returning a result (e.g. **Task&lt;T&gt;.Result** or **await**). For **Thread**, the shared-variable + Join pattern is the standard approach.

---

## Summary table

| Concept | Role |
|--------|------|
| **Thread and results** | No built-in return; use shared state + completion sync. |
| **Shared variable** | Worker writes result; reader reads after Join (or equivalent). |
| **Join** | Blocks until thread has finished; then safe to read worker’s writes. |
| **Task** | Provides first-class support for return values later in the course. |

---

*Professional summary of the Returning results from a thread video.*
