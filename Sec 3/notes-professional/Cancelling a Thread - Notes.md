# Notes: Cancelling a Thread (Professional)

*From `5. Cancelling a thread.txt` + `Canceling+Threads.txt`.*

---

## 1. Motivation

Once a worker thread is started, it is independent of the main thread. The main thread may remain responsive (e.g. UI), but the user may still be waiting for the worker’s result. If the operation is long-running (e.g. a slow remote call), the user may want to **cancel**. So we need a way to signal the worker to stop. With **Thread**, there is no built-in “cancel” method; cancellation must be **cooperative**: the worker periodically checks a flag (or token) and exits when cancellation is requested.

---

## 2. Shared cancellation flag

A simple approach is a **shared boolean** (e.g. `volatile bool cancelThread` or a variable accessed under appropriate synchronization). The worker loop (or long-running logic) **polls** this flag. When the user (or main thread) decides to cancel, it sets the flag to true. The worker sees it on a subsequent check and exits the loop (or method) in a controlled way. No forced termination of the thread; the worker must cooperate.

---

## 3. Example

The lesson uses a worker that runs a long loop with **Thread.SpinWait** to simulate work. Inside the loop it checks a shared `cancelThread` (or similar). The main thread prompts the user (e.g. “press ‘c’ to cancel”), reads console input, and if the user cancels, sets the flag. The worker breaks out of the loop and may print a message (e.g. iteration at which cancellation was requested). Whether to print “Work is done” only when not cancelled is a design choice (e.g. only when the loop completes normally).

---

## 4. CancellationTokenSource / CancellationToken

The lesson mentions that .NET provides **CancellationTokenSource** and **CancellationToken** for cancellation. The worker checks **token.IsCancellationRequested** (or throws via **token.ThrowIfCancellationRequested()**). This is the standard mechanism and is used extensively with **Task** and async APIs. For this lesson the focus is the simple shared-flag pattern so the idea is clear; cancellation tokens are covered in more depth when discussing Task.

---

## Summary table

| Concept | Role |
|--------|------|
| **Cooperative cancellation** | Worker must check a flag (or token) and exit; no forced abort. |
| **Shared flag** | Main thread (or other) sets flag; worker polls and breaks. |
| **CancellationToken** | Standard .NET mechanism; used with Task and async. |
| **Join after cancel** | Main thread can still Join to ensure worker has finished. |

---

*Professional summary of the Cancelling a thread video and code.*
