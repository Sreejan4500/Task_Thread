# Notes: Thread Affinity (Professional)

*From `10. Thread Affinity.txt` + `Thread+Affinity+(Windows+Forms).txt` and `Thread+Affinity+(Blazor).txt`.*

---

## 1. Definition and scope

- **Thread affinity** means that a resource is associated with a specific thread; only that thread should access or modify it.
- We have already seen this with synchronization primitives: locks acquired by a thread must be released by the same thread (Monitor, lock, Mutex, reader/writer locks). Semaphore does **not** have this requirement—one thread can wait and another can release.
- Thread affinity applies beyond locks: **UI controls** and other context-bound resources are typically tied to the thread that created them (e.g. the UI thread). Accessing them from another thread can cause cross-thread operation exceptions or undefined behavior.

---

## 2. Windows Forms: cross-thread access to controls

In the offloading demo, worker threads call code that updates a label:

```csharp
lblMessage.Text = message;
```

The label was created on the **main (UI) thread**. When a **worker thread** sets its `Text`, the runtime can throw: **“Cross-thread operation not valid: Control ‘lblMessage’ accessed from a thread other than the thread it was created on.”** In the lesson, this appears when running under the debugger; behavior without the debugger may vary, but the pattern is still incorrect.

So UI controls have thread affinity: they must be accessed only from the thread that owns the control’s underlying window handle.

---

## 3. Fix: Invoke to marshal to the owning thread

To update a control from another thread, the work must run **on the thread that owns the control**. Use **Invoke** (or BeginInvoke) on the control:

```csharp
lblMessage.Invoke(() =>
{
    lblMessage.Text = message;
});
```

The delegate is executed on the thread that owns the control. That “synchronizes context” between the worker thread and the UI thread so the control is only ever touched by its owner.

Documentation: the delegate is executed on the thread that owns the control’s underlying window handle.

---

## 4. InvokeRequired

To avoid unnecessary marshaling when the code might already run on the UI thread:

```csharp
if (lblMessage.InvokeRequired)
{
    lblMessage.Invoke(() => { lblMessage.Text = message; });
}
else
{
    lblMessage.Text = message;
}
```

- **InvokeRequired** is true when the current thread is **not** the thread that created the control, so Invoke is needed.
- When false, the caller is already on the UI thread and can update the control directly. This keeps the same method safe whether called from the UI thread or a worker thread.

---

## 5. Blazor: synchronization context and StateHasChanged

In Blazor (e.g. server interactive), the UI has a synchronization context. If a **different thread** updates component state and then calls **StateHasChanged()**, you can hit thread-affinity issues: the update may not render correctly, or you may see reconnection/errors in the browser.

**Fix:** run the UI update on the correct context using **InvokeAsync**:

```csharp
void DisplayTime()
{
    Thread thread = new Thread(() =>
    {
        Thread.Sleep(500);
        base.InvokeAsync(() =>
        {
            currentTime = DateTime.Now.ToString();
            StateHasChanged();
        });
    });
    thread.Start();
}
```

The worker thread does not touch component state or call StateHasChanged directly; it queues the update and refresh to the component’s synchronization context. The lesson notes that thread affinity will reappear when discussing **async and await**.

---

## 6. Takeaway

- Many resources have thread affinity; UI is a central example.
- From a non-owning thread, use **Invoke** (Windows Forms) or **InvokeAsync** (Blazor) so the update runs on the correct thread/context.
- This is coordination of execution context, not just locking shared data.

---

## Summary table

| Concept | Role |
|--------|------|
| **Thread affinity** | Resource is bound to a specific thread; only that thread should access it. |
| **UI controls** | Created on UI thread; must be updated only on that thread. |
| **Invoke (WinForms)** | Run a delegate on the thread that owns the control. |
| **InvokeRequired** | True when current thread is not the control’s owner; use Invoke. |
| **InvokeAsync (Blazor)** | Run an action on the component’s sync context for safe UI updates. |

---

*Professional summary of the Thread Affinity video and WinForms/Blazor code.*
