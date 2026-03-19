# Notes: Multithreading vs Async Programming (Professional)

*From transcript "Multithreading vs Async Programming".*

---

## 1. Same mechanism, different emphasis

**From the transcript:**  
> “I have to say that they're the same thing, except they have different emphasis from the basic syntax. They both look very similar, and actually in most cases they do both utilize threads to accomplish their work.”

- **Multithreading** and **asynchronous programming** both rely on **threads**: work can run on threads other than the main thread, and the OS scheduler assigns those threads to CPUs. The underlying execution model is the same.
- The difference is **emphasis** and **typical use case**, not “one uses threads and the other doesn’t.” Async code in .NET (e.g. `async`/`await`) still uses threads (or thread-pool threads) for completion; the mental model and API stress “don’t block the calling thread” and “run work aside.”

---

## 2. Multithreading: emphasis on divide and conquer

- **Multithreading** emphasizes **splitting a large task** into chunks and having **multiple threads** work on those chunks **in parallel** to finish sooner. The goal is **throughput** and **performance**: use more CPU (or more cores) to get the job done faster.
- You may have many threads (e.g. one per segment, or a pool) all doing CPU work at the same time. Example: summing a large array by splitting it across four threads.

**From the transcript:**  
> “Multi-threading programming emphasis the divide and conquer aspect … you could have hundreds of threads running at the same time because you want to divide the task and then conquer them so that they can be finished in parallel. So the performance is going to be improved greatly.”

- So: multithreading **stresses** “many threads working side by side to solve the problem faster.”

---

## 3. Asynchronous programming: emphasis on offloading

- **Asynchronous programming** emphasizes **moving a long-running task off the current (e.g. main) thread** so that thread is **not blocked**. One (or more) other threads do the work “on the side”; the calling thread can continue (e.g. stay responsive) or later wait for the result. The goal is **responsiveness** and **non-blocking** behavior.
- It’s still multithreading (main thread + worker thread(s)), but the **focus** is “something running aside from the main thread” so the main thread doesn’t sit and wait.

**From the transcript:**  
> “Asynchronous programming emphasizes … the second reason, which is to offload the long running task. So we have this main thread going and then we have this big fat task. So we want to move this big fat task to the side so that it can be accomplished in another thread … It's just that asynchronous emphasize on the fact that there is something that is running aside from the main thread. This is to offload the long running task.”

- So: async **stresses** “offload work so the main thread isn’t blocked; work runs aside.”

---

## 4. CPU-bound vs I/O-bound

- **CPU-bound:** The task mainly uses the **CPU** (lots of computation: math, loops, encoding, etc.). To speed it up, you add more CPU capacity: more threads (or more cores) doing compute work in parallel. **Multithreading** (divide and conquer) is a natural fit: e.g. games, image processing, simulations.

**From the transcript:**  
> “Multi-threading programming is often used for CPU bound operations, and the operation that we'll use a lot of CPU resources to calculate, for example, games.”

- **I/O-bound:** The task spends most of its time **waiting** on something outside the CPU: network, disk, database, API. The thread is often idle (blocked or waiting); it doesn’t need much CPU. You want to **not block** the main thread while waiting, and ideally use few threads (e.g. thread-pool) so many I/O operations can be in flight without one thread per operation. **Asynchronous programming** is a natural fit: e.g. HTTP calls, file I/O, database queries.

**From the transcript:**  
> “Asynchronous programming is particularly useful for I/O bound operations. Why is that? Because this big fat task doesn't use much CPU. We just have to sit and wait for something to return. Mostly it's from a remote server, a remote database … Therefore, it takes a very long time. That's what we call an I/O operation. It doesn't take much CPU resource, but it's just remote and it's a hardware device. Therefore it takes time to complete. We want to put it in another thread so that it runs asynchronously aside from the main thread.”

- So: **CPU-bound → multithreading** (parallel compute); **I/O-bound → async** (offload waiting, keep UI/main thread free).

---

## 5. Summary: one model, two emphases

| Aspect | Multithreading | Asynchronous programming |
|--------|----------------|---------------------------|
| **Uses threads?** | Yes | Yes (worker/thread-pool threads) |
| **Emphasis** | Divide and conquer; many threads in parallel for **performance** | Offload long-running work; work runs **aside** so main thread isn’t blocked |
| **Typical use** | CPU-bound (games, heavy computation, parallel algorithms) | I/O-bound (network, disk, DB, APIs); responsiveness |
| **Goal** | Finish the big task faster (throughput) | Don’t block; stay responsive; wait without tying up a thread |

**From the transcript:**  
> “Only difference is the emphasis.”

---

*Same structure as other professional notes; one file per transcript/topic.*
