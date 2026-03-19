# Course Roadmap: Master Multithreading & Asynchronous Programming in C#/.NET

**Study budget:** 1–1.5 hours per day  
**Total course video:** ~8h 30m (69 lectures, 9 sections)

This roadmap is based on:
- **Content already in this repo:** Sections 1–4 (transcripts + notes-kid + notes-professional).
- **Content assumed to be added later:** Sections 5–9 (Async/Await, Parallel Loops, PLINQ, Concurrent Collections, Bonus).

---

## How the time estimates work

- **Video duration** = raw lecture length from the Udemy course.
- **Study time** = video + reading transcripts + reviewing/using your notes. Assumed **~1.3–1.5×** video time when you use both transcript and notes.
- **Assignments** = extra time for Web Server (Assignment 1) and Airplane Seats (Assignment 2); you already have Assignment 2 in the repo.

---

## Section-by-section plan

| Section | Topic | Lectures | Video | Study time (est.) | Status in repo |
|--------|--------|----------|-------|-------------------|----------------|
| **1** | Introduction | 7 | 1h 05m | ~1.5–2 h | ✅ Present |
| **2** | Threads Synchronization | 16 | 2h 44m | ~4–5 h | ✅ Present |
| **3** | Multithreading MISC | 7 | 51m | ~1–1.5 h | ✅ Present |
| **4** | Task-based Async Programming | 14 | 1h 10m | ~2–2.5 h | ✅ Present |
| **5** | Async & Await | 7 | 43m | ~1–1.5 h | ⏳ To be added |
| **6** | Parallel Loops | 9 | 1h 02m | ~1.5–2 h | ⏳ To be added |
| **7** | PLINQ | 5 | 33m | ~0.75–1 h | ⏳ To be added |
| **8** | Concurrent Collections | 3 | 23m | ~0.5–0.75 h | ⏳ To be added |
| **9** | BONUS | 1 | 1m | — | ⏳ To be added |
| — | **Assignments** (Web Server + Airplane Seats) | — | — | ~2–3 h | Assignment 2 ✅ |

**Rough totals:**
- Video only: **8h 30m**
- Study (video + notes/transcripts): **~12–15 hours**
- With assignments: **~14–18 hours**

---

## Suggested timeline at 1–1.5 hours/day

Assuming **~1.25 hours/day** on average:

| Phase | Sections | Estimated days | Cumulative |
|-------|----------|----------------|------------|
| **1** | Sec 1 (Introduction) | 1–2 days | Day 2 |
| **2** | Sec 2 (Thread Synchronization) | 4–5 days | Day 7 |
| **3** | Sec 3 (Multithreading MISC) | 1 day | Day 8 |
| **4** | Sec 4 (Task-based Async) | 2–3 days | Day 11 |
| **5** | Assignment 1 (Web Server) or Assignment 2 (Airplane Seats) | 1–2 days | Day 13 |
| **6** | Sec 5 (Async & Await) | 1–2 days | Day 15 |
| **7** | Sec 6 (Parallel Loops) | 1.5–2 days | Day 17 |
| **8** | Sec 7 (PLINQ) | ~1 day | Day 18 |
| **9** | Sec 8 (Concurrent Collections) + Sec 9 (Bonus) | ~1 day | Day 19 |
| **10** | Buffer / review / second assignment | 2–3 days | Day 21–22 |

**Ideal completion: about 3 weeks** (15–22 days at 1–1.5 hrs/day), depending on how deep you go on sync (Sec 2) and tasks (Sec 4).

---

## Weekly view (example)

| Week | Focus | Goal |
|------|--------|------|
| **Week 1** | Sec 1 + Sec 2 (through ~half of sync) | Intro done; Critical Section, Exclusive Lock, Monitor, Mutex, Reader/Writer, Semaphore, AutoResetEvent, ManualResetEvent, Affinity, Safety, Deadlocks |
| **Week 2** | Rest of Sec 2 (if needed) + Sec 3 + Sec 4 | MISC done; Task-based async done (Thread vs Task, Task syntax, continuations, exception, cancellation, sync) |
| **Week 3** | Assignments + Sec 5–9 | One assignment done; Async/Await, Parallel Loops, PLINQ, Concurrent Collections, Bonus; buffer for second assignment or review |

---

## Content already in this folder (for your checklist)

- **Sec 1:** CPU/Thread Scheduler, Basic Syntax, Divide & Conquer, Offload Long Running Tasks (+ Assignment 1 intro).
- **Sec 2:** Thread Sync Overview, Critical Section & Atomic Op, Exclusive Locks, Monitor, Mutex, Reader/Writer Locks, Semaphore, AutoResetEvent, ManualResetEvent, Thread Affinity, Thread Safety, Nested Locks & Deadlocks.
- **Sec 3:** Debug with Multiple Threads, States of a Thread, Make Thread Wait, Returning Results, Cancelling a Thread, Thread Pool, Exception Handling in Threads.
- **Sec 4:** Multithreading vs Async, Thread vs Task, Basic Task Syntax, Task Uses Thread Pool, Returning Result, ContinueWith, Wait/WaitAll/Result, WhenAny/WhenAll, Continuation Chain & Unwrap, Exception in Task, Task Cancellation, Tasks Synchronization.
- **Assignments:** Assignment 2 (e.g. airplane seats) project present; Assignment 1 (Web Server) to be done when you have the material.

---

## When Sec 5–9 content is added

- **Sec 5 (Async & Await):** ~1–2 days at your pace.
- **Sec 6 (Parallel Loops):** ~1.5–2 days.
- **Sec 7 (PLINQ):** ~1 day.
- **Sec 8 (Concurrent Collections) + Sec 9 (Bonus):** ~0.5–1 day.

You can paste this block into ROADMAP.md under a “Future sections” heading and tick off as you go:

```text
[ ] Sec 5 – Async & Await
[ ] Sec 6 – Parallel Loops
[ ] Sec 7 – PLINQ
[ ] Sec 8 – Concurrent Collections
[ ] Sec 9 – BONUS
```

---

**Summary:** At **1–1.5 hours per day**, plan for **about 3 weeks** to complete the course and one assignment comfortably, with a few buffer days for review or a second assignment. Section 2 (Thread Synchronization) is the longest; spread it over 4–5 days if needed.
