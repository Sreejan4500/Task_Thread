# Notes: Why Threading — Divide and Conquer (Professional)

*From transcript "Why Threading - Divide & Conquer" + Divide+and+Conquer code.*

---

## 1. Why start new threads: divide and conquer

- One main reason to use multiple threads is to solve **divide and conquer** problems: a single large task is split into **chunks** (segments), and each chunk is processed by a **separate thread** in parallel. Partial results are then combined (e.g. summed) to get the final result.
- Conceptually: one big rectangle (task) → split into sections → assign each section to a thread → run in parallel → merge results.

**From the transcript:**  
> “You have a huge task … we can actually divide this task into different chunks or different sections so that we can let multiple people working on this task.”

---

## 2. Real-world analogy and when it helps

- **Analogy:** Cooking four dishes — one person does all four (serial) vs two people each doing two (parallel). Same total work, but wall-clock time can be roughly halved.
- **In code:** Same idea. A task that can be split into independent chunks (e.g. summing different parts of an array) can be assigned to multiple threads so the work runs in parallel and **total elapsed time** decreases, even though total CPU work is similar or slightly higher (thread creation, joining).

---

## 3. Example: summing an array (single-thread baseline)

- **Task:** Sum an array of integers (e.g. 10 elements). Single-thread approach: one loop, one accumulator, iterate over the whole array.
- To simulate **CPU-intensive** work per element (e.g. for later comparison), the demo uses **`Thread.Sleep(100)`** per iteration. For 10 elements that’s ~1000 ms total.
- **Timing:** Capture `startTime = DateTime.Now` before the loop and `endTime = DateTime.Now` after; report `(endTime - startTime).TotalMilliseconds`.

```csharp
int sum = 0;
foreach (var number in array)
{
    Thread.Sleep(100);  // simulate heavy work per element
    sum += number;
}
// sum == 55 for { 1, 2, ..., 10 }
```

---

## 4. Divide and conquer: split array into segments

- **Idea:** Divide the array into **N** contiguous segments (e.g. N = 4). Each of **N** threads computes the sum of **one** segment. Main thread (or same thread after joining) adds the **N** partial sums to get the total.
- **Segment layout:**  
  - `segmentLength = array.Length / numOfThreads`  
  - Thread 0: indices `[0, segmentLength)`  
  - Thread 1: `[segmentLength, 2 * segmentLength)`  
  - Thread 2: `[2 * segmentLength, 3 * segmentLength)`  
  - Thread 3: `[3 * segmentLength, array.Length)` (last segment takes remainder so length doesn’t need to divide evenly).

- **Method:** A single helper that sums one segment and returns the value:

  ```csharp
  int SumSegment(int start, int end)
  {
      int segmentSum = 0;
      for (int i = start; i < end; i++)
      {
          Thread.Sleep(100);
          segmentSum += array[i];
      }
      return segmentSum;
  }
  ```

---

## 5. Assigning work to threads (lambdas and captured variables)

- **`Thread`** constructor accepts a **delegate** (e.g. `ThreadStart` = `void` method with no parameters). To pass arguments and **get a return value**, use a **lambda** that closes over variables:
  - Each thread runs a lambda that calls `SumSegment` with the right `(start, end)` and assigns the result to a variable (e.g. `sum1`, `sum2`, `sum3`, `sum4`).
- **Important:** Those variables must be **initialized** (e.g. `int sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0`) before starting threads, and must be **assigned only by the thread** that owns that segment (no sharing of accumulators between threads in this pattern).

```csharp
int sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0;
int numofThreads = 4;
int segmentLength = array.Length / numofThreads;

Thread[] threads = new Thread[numofThreads];
threads[0] = new Thread(() => { sum1 = SumSegment(0, segmentLength); });
threads[1] = new Thread(() => { sum2 = SumSegment(segmentLength, 2 * segmentLength); });
threads[2] = new Thread(() => { sum3 = SumSegment(2 * segmentLength, 3 * segmentLength); });
threads[3] = new Thread(() => { sum4 = SumSegment(3 * segmentLength, array.Length); });
```

---

## 6. Start and Join: non-blocking start, blocking wait

- **`thread.Start()`** is **non-blocking**: it returns immediately; the thread runs asynchronously. If you read `sum1`…`sum4` or take `endTime` right after starting all threads, the threads may not have finished and the partial sums would be wrong (or still zero).
- **`thread.Join()`** blocks the **calling thread** until **that** thread has terminated. So: `foreach (var thread in threads) thread.Join();` ensures all four threads have finished before the main thread continues.
- **Correct ordering:**  
  1. `startTime = DateTime.Now`  
  2. Start all threads  
  3. Join all threads  
  4. `endTime = DateTime.Now`  
  5. Total sum = `sum1 + sum2 + sum3 + sum4`  
  6. Report time and result.

**From the transcript:**  
> “When we use thread it starts asynchronously which means that it's not blocking … to get the actual time, we will need to wait … We're going to do … thread dot join … that blocks the calling thread until the thread … terminates.”

---

## 7. Observed performance (demo scenario)

- With **10 elements** and **100 ms sleep per element**:
  - **Single thread:** ~1000 ms (10 × 100 ms).
  - **Four threads:** ~400 ms (e.g. segments of 2–3 elements each; each thread does 200–300 ms of work; they run in parallel, so wall clock is roughly the slowest segment plus overhead).
- For **very small** arrays (e.g. 10 elements), creating 4 threads adds overhead and may not be worth it. The pattern is intended for **large** arrays or expensive per-element work (or real CPU-bound work without sleep), where splitting across threads reduces **elapsed time**.

**From the transcript:**  
> “In real life scenarios, you might have thousands of elements in an array, and spinning up multiple threads like this is going to definitely cut down the calculation time.”

---

## 8. Full example (from Divide+and+Conquer code)

```csharp
int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

int SumSegment(int start, int end)
{
    int segmentSum = 0;
    for (int i = start; i < end; i++)
    {
        Thread.Sleep(100);
        segmentSum += array[i];
    }
    return segmentSum;
}

int sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0;
var startTime = DateTime.Now;

int numofThreads = 4;
int segmentLength = array.Length / numofThreads;

Thread[] threads = new Thread[numofThreads];
threads[0] = new Thread(() => { sum1 = SumSegment(0, segmentLength); });
threads[1] = new Thread(() => { sum2 = SumSegment(segmentLength, 2 * segmentLength); });
threads[2] = new Thread(() => { sum3 = SumSegment(2 * segmentLength, 3 * segmentLength); });
threads[3] = new Thread(() => { sum4 = SumSegment(3 * segmentLength, array.Length); });

foreach (var thread in threads) { thread.Start(); }
foreach (var thread in threads) { thread.Join(); }

var endTime = DateTime.Now;
var timespan = endTime - startTime;

Console.WriteLine($"The sum is {sum1 + sum2 + sum3 + sum4}");
Console.WriteLine($"The time it takes: {timespan.TotalMilliseconds}");
Console.ReadLine();
```

---

## Summary table

| Concept | Role |
|--------|------|
| **Divide and conquer** | Split one big task into chunks; assign each chunk to a thread; combine partial results. |
| **Segment** | Contiguous range of indices (e.g. `[start, end)`) assigned to one thread. |
| **SumSegment(start, end)** | Returns sum of `array[start..end)`; same per-element cost (e.g. Sleep) as single-thread loop. |
| **Lambda + captured variables** | Pass arguments and receive “return” value via closure (e.g. `sum1 = SumSegment(...)`). |
| **Start()** | Begins thread execution; non-blocking; returns immediately. |
| **Join()** | Blocks calling thread until that thread terminates; use after Start() to wait for all before reading results and timing. |
| **Timing** | Take start before Start(), end after Join(); report `(end - start).TotalMilliseconds`. |
| **When it helps** | Large data or expensive per-item work; multiple threads can reduce wall-clock time. |

---

*Same structure as other professional notes in this section; one file per transcript/topic.*
