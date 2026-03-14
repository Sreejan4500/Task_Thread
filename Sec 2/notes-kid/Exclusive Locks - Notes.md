# Notes: Exclusive Locks (Kid-Friendly)

*From `3. Exclusive Locks.txt` + `3. Exclusive+Lock.txt`, explained simply.*

---

## 1. What is an exclusive lock?

An **exclusive lock** means:

- only **one thread** can enter a protected section at a time,
- all other threads must wait.

In C#, the common syntax is:

```csharp
lock (counterLock)
{
    counter = counter + 1;
}
```

- **To a kid:**  
  It is like putting one key on the door. Only one worker can take the key, go inside, do the job, and then return the key.

---

## 2. Why we need it

Before using `lock`, this line caused trouble:

```csharp
counter = counter + 1;
```

because two threads could do it at the same time and lose updates.

By putting it inside a `lock`, we make sure the dangerous section is used by one thread at a time.

That protects the shared counter.

---

## 3. The lock object

To use `lock`, we first need an object to lock on:

```csharp
object counterLock = new object();
```

Then we use it:

```csharp
lock (counterLock)
{
    counter = counter + 1;
}
```

The lock object is not special by itself. It is just the token all threads agree to use.

- **Own note:**  
  Good practice is to use a dedicated object only for locking, so unrelated code does not accidentally lock on the same object.

---

## 4. Full example from the lesson

```csharp
int counter = 0;
object counterLock = new object();

Thread thread1 = new Thread(IncrementCounter);
Thread thread2 = new Thread(IncrementCounter);

thread1.Start();
thread2.Start();

thread1.Join();
thread2.Join();

Console.WriteLine($"Final counter value is: {counter}");

void IncrementCounter()
{
    for (int i = 0; i < 100000; i++)
    {
        lock (counterLock)
        {
            counter = counter + 1;
        }
    }
}
```

Now the result becomes consistent.

Expected final value:

- `200,000`

---

## 5. Why this works

Even though `counter = counter + 1` is not truly atomic by itself, the `lock` makes it behave safely.

That is because:

- thread 1 enters the lock,
- thread 2 must wait,
- thread 1 finishes the increment,
- thread 1 leaves the lock,
- then thread 2 may enter.

So the dangerous code is no longer happening at the same moment in both threads.

---

## 6. Important result from the demo

The teacher shows that once the lock is added, the counter becomes correct every time.

If each thread adds `1` a hundred thousand times, then together they should produce:

- `200,000`

- **Own note:**  
  The transcript briefly says `300,000` once, but based on the actual code and loop counts, the correct expected result is `200,000`.

---

## 7. What happens if code inside the lock throws an error?

The lesson says the `lock` keyword already uses a built-in safety pattern similar to `try/finally`.

That means:

- if something goes wrong inside the lock,
- the lock still gets released properly.

So you usually do not need to manually release it when using the `lock` keyword.

- **To a kid:**  
  Even if a worker trips inside the room, the room key still gets returned instead of getting stuck forever.

---

## 8. Why `lock` is easy to like

The teacher says `lock` is simple and convenient.

That is because:

- the syntax is short,
- the code is easy to read,
- it protects a critical section clearly,
- release happens automatically.

So for many same-process situations, `lock` is a very friendly tool.

---

## 9. Version note from the lesson

The video mentions:

- older C# versions commonly use `object` as the lock object,
- newer versions introduce dedicated lock-related improvements.

But the big teaching point is unchanged:

- use a dedicated reference object,
- use it only to protect the critical section.

---

## 10. Main limitation

A normal `lock` works **inside one process**.

It is great when threads inside the same app share data.

It is not the right tool for coordinating multiple different processes across the operating system.

That is why later lessons introduce `Mutex`.

---

## Quick recap (kid version)

| Thing | Simple idea |
|------|-------------|
| **Exclusive lock** | Only one thread can enter the protected code at a time. |
| **`lock (...) { ... }`** | C# way to protect a critical section simply. |
| **Lock object** | The shared "key" all threads use. |
| **Why it works** | Threads must take turns around shared data. |
| **Result** | The counter becomes correct and consistent. |
| **Extra safety** | `lock` releases properly even if an exception happens. |

---

*This note combines the explanation and the code example into one kid-friendly summary.*
