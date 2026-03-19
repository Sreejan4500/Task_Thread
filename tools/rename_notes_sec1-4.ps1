$ErrorActionPreference = 'Stop'

$base = Split-Path -Parent $PSScriptRoot

function Rename-NotesFiles {
    param(
        [Parameter(Mandatory = $true)]
        [array] $Mappings
    )

    foreach ($m in $Mappings) {
        $old = Join-Path $base $m.Old
        $new = Join-Path $base $m.New

        if (-not (Test-Path -LiteralPath $old)) {
            throw "Missing source file: $($m.Old)"
        }
        if (Test-Path -LiteralPath $new) {
            throw "Target already exists: $($m.New)"
        }
    }

    foreach ($m in $Mappings) {
        $old = Join-Path $base $m.Old
        $leaf = Split-Path -Leaf $m.New
        Rename-Item -LiteralPath $old -NewName $leaf
    }
}

$sec1 = @(
    @{ Old = 'Sec 1/notes-kid/CPU, Thread and Thread Scheduler - Notes.md'; New = 'Sec 1/notes-kid/1. CPU, Thread and Thread Scheduler - Notes.md' },
    @{ Old = 'Sec 1/notes-professional/CPU, Thread and Thread Scheduler - Notes.md'; New = 'Sec 1/notes-professional/1. CPU, Thread and Thread Scheduler - Notes.md' },
    @{ Old = 'Sec 1/notes-kid/Basic Syntax to Start a Thread - Notes.md'; New = 'Sec 1/notes-kid/2. Basic Syntax to Start a Thread - Notes.md' },
    @{ Old = 'Sec 1/notes-professional/Basic Syntax to Start a Thread - Notes.md'; New = 'Sec 1/notes-professional/2. Basic Syntax to Start a Thread - Notes.md' },
    @{ Old = 'Sec 1/notes-kid/Why Threading - Divide and Conquer - Notes.md'; New = 'Sec 1/notes-kid/3. Why Threading - Divide & Conquer - Notes.md' },
    @{ Old = 'Sec 1/notes-professional/Why Threading - Divide and Conquer - Notes.md'; New = 'Sec 1/notes-professional/3. Why Threading - Divide & Conquer - Notes.md' },
    @{ Old = 'Sec 1/notes-kid/Why Threading - Offload Long Running Tasks - Notes.md'; New = 'Sec 1/notes-kid/4. Why Threading - Offload Long Running Tasks - Notes.md' },
    @{ Old = 'Sec 1/notes-professional/Why Threading - Offload Long Running Tasks - Notes.md'; New = 'Sec 1/notes-professional/4. Why Threading - Offload Long Running Tasks - Notes.md' }
)

$sec2 = @(
    @{ Old = 'Sec 2/notes-kid/Thread Synchronization - Notes.md'; New = 'Sec 2/notes-kid/1. Thread Synchronization Overview - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/Thread Synchronization - Notes.md'; New = 'Sec 2/notes-professional/1. Thread Synchronization Overview - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/Critical Section and Atomic Operation - Notes.md'; New = 'Sec 2/notes-kid/2. Critical Section and Atomic Operation - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/Critical Section and Atomic Operation - Notes.md'; New = 'Sec 2/notes-professional/2. Critical Section and Atomic Operation - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/Exclusive Locks - Notes.md'; New = 'Sec 2/notes-kid/3. Exclusive Locks - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/Exclusive Locks - Notes.md'; New = 'Sec 2/notes-professional/3. Exclusive Locks - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/Monitor - Notes.md'; New = 'Sec 2/notes-kid/4. Use Monitor to add timeout for locks - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/Monitor - Notes.md'; New = 'Sec 2/notes-professional/4. Use Monitor to add timeout for locks - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/Mutex - Notes.md'; New = 'Sec 2/notes-kid/5. Use Mutex to synchronize across processes - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/Mutex - Notes.md'; New = 'Sec 2/notes-professional/5. Use Mutex to synchronize across processes - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/Reader and Writer Locks - Notes.md'; New = 'Sec 2/notes-kid/6. Reader and Writer Locks - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/Reader and Writer Locks - Notes.md'; New = 'Sec 2/notes-professional/6. Reader and Writer Locks - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/Semaphore - Notes.md'; New = 'Sec 2/notes-kid/7. Use Semaphore to limit number of threads - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/Semaphore - Notes.md'; New = 'Sec 2/notes-professional/7. Use Semaphore to limit number of threads - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/AutoResetEvent - Notes.md'; New = 'Sec 2/notes-kid/8. Use AutoResetEvent for signaling - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/AutoResetEvent - Notes.md'; New = 'Sec 2/notes-professional/8. Use AutoResetEvent for signaling - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/ManualResetEvent - Notes.md'; New = 'Sec 2/notes-kid/9. Use ManualResetEvent to manually release multiple threads - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/ManualResetEvent - Notes.md'; New = 'Sec 2/notes-professional/9. Use ManualResetEvent to manually release multiple threads - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/Thread Affinity - Notes.md'; New = 'Sec 2/notes-kid/10. Thread Affinity - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/Thread Affinity - Notes.md'; New = 'Sec 2/notes-professional/10. Thread Affinity - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/Thread Safety - Notes.md'; New = 'Sec 2/notes-kid/11. Thread Safety - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/Thread Safety - Notes.md'; New = 'Sec 2/notes-professional/11. Thread Safety - Notes.md' },

    @{ Old = 'Sec 2/notes-kid/Nested Locks and Deadlocks - Notes.md'; New = 'Sec 2/notes-kid/12. Nested Locks and Deadlocks - Notes.md' },
    @{ Old = 'Sec 2/notes-professional/Nested Locks and Deadlocks - Notes.md'; New = 'Sec 2/notes-professional/12. Nested Locks and Deadlocks - Notes.md' }
)

$sec3 = @(
    @{ Old = 'Sec 3/notes-kid/Debug Programs with Multiple Threads - Notes.md'; New = 'Sec 3/notes-kid/1. Debug programs with multiple threads - Notes.md' },
    @{ Old = 'Sec 3/notes-professional/Debug Programs with Multiple Threads - Notes.md'; New = 'Sec 3/notes-professional/1. Debug programs with multiple threads - Notes.md' },

    @{ Old = 'Sec 3/notes-kid/States of a Thread - Notes.md'; New = 'Sec 3/notes-kid/2. States of a thread - Notes.md' },
    @{ Old = 'Sec 3/notes-professional/States of a Thread - Notes.md'; New = 'Sec 3/notes-professional/2. States of a thread - Notes.md' },

    @{ Old = 'Sec 3/notes-kid/Make Thread Wait for Some Time - Notes.md'; New = 'Sec 3/notes-kid/3. Make thread wait for some time - Notes.md' },
    @{ Old = 'Sec 3/notes-professional/Make Thread Wait for Some Time - Notes.md'; New = 'Sec 3/notes-professional/3. Make thread wait for some time - Notes.md' },

    @{ Old = 'Sec 3/notes-kid/Returning Results from a Thread - Notes.md'; New = 'Sec 3/notes-kid/4. Returning results from a thread - Notes.md' },
    @{ Old = 'Sec 3/notes-professional/Returning Results from a Thread - Notes.md'; New = 'Sec 3/notes-professional/4. Returning results from a thread - Notes.md' },

    @{ Old = 'Sec 3/notes-kid/Cancelling a Thread - Notes.md'; New = 'Sec 3/notes-kid/5. Cancelling a thread - Notes.md' },
    @{ Old = 'Sec 3/notes-professional/Cancelling a Thread - Notes.md'; New = 'Sec 3/notes-professional/5. Cancelling a thread - Notes.md' },

    @{ Old = 'Sec 3/notes-kid/Thread Pool - Notes.md'; New = 'Sec 3/notes-kid/6. Thread Pool - Notes.md' },
    @{ Old = 'Sec 3/notes-professional/Thread Pool - Notes.md'; New = 'Sec 3/notes-professional/6. Thread Pool - Notes.md' },

    @{ Old = 'Sec 3/notes-kid/Exception Handling in Threads - Notes.md'; New = 'Sec 3/notes-kid/7. Exception Handling in Threads - Notes.md' },
    @{ Old = 'Sec 3/notes-professional/Exception Handling in Threads - Notes.md'; New = 'Sec 3/notes-professional/7. Exception Handling in Threads - Notes.md' }
)

$sec4 = @(
    @{ Old = 'Sec 4/notes-kid/Multithreading vs Async Programming - Notes.md'; New = 'Sec 4/notes-kid/1. Multithreading vs Async Programming - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Multithreading vs Async Programming - Notes.md'; New = 'Sec 4/notes-professional/1. Multithreading vs Async Programming - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Basic Syntax of using Task - Notes.md'; New = 'Sec 4/notes-kid/2. Basic Syntax of using Task - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Basic Syntax of using Task - Notes.md'; New = 'Sec 4/notes-professional/2. Basic Syntax of using Task - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Thread vs Task - Notes.md'; New = 'Sec 4/notes-kid/3. Thread vs Task - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Thread vs Task - Notes.md'; New = 'Sec 4/notes-professional/3. Thread vs Task - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Task Uses Thread Pool by Default - Notes.md'; New = 'Sec 4/notes-kid/4. Task uses Thread Pool by default - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Task Uses Thread Pool by Default - Notes.md'; New = 'Sec 4/notes-professional/4. Task uses Thread Pool by default - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Returning Result from a Task - Notes.md'; New = 'Sec 4/notes-kid/5. Returning result from a task - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Returning Result from a Task - Notes.md'; New = 'Sec 4/notes-professional/5. Returning result from a task - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Task Continuation - Wait, WaitAll, Result - Notes.md'; New = 'Sec 4/notes-kid/6. Task Continuation - Wait, WaitAll, Result - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Task Continuation - Wait, WaitAll, Result - Notes.md'; New = 'Sec 4/notes-professional/6. Task Continuation - Wait, WaitAll, Result - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Task Continuation - ContinueWith - Notes.md'; New = 'Sec 4/notes-kid/7. Task Continuation - ContinueWith - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Task Continuation - ContinueWith - Notes.md'; New = 'Sec 4/notes-professional/7. Task Continuation - ContinueWith - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Task Continuation - WhenAny, WhenAll - Notes.md'; New = 'Sec 4/notes-kid/8. Task Continuation - WhenAny, WhenAll - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Task Continuation - WhenAny, WhenAll - Notes.md'; New = 'Sec 4/notes-professional/8. Task Continuation - WhenAny, WhenAll - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Task Continuation - Continuation Chain and Unwrap - Notes.md'; New = 'Sec 4/notes-kid/9. Task Continuation - Continuation Chain and Unwrap - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Task Continuation - Continuation Chain and Unwrap - Notes.md'; New = 'Sec 4/notes-professional/9. Task Continuation - Continuation Chain and Unwrap - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Exception Handling in Task - Notes.md'; New = 'Sec 4/notes-kid/10. Exception Handling in Task - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Exception Handling in Task - Notes.md'; New = 'Sec 4/notes-professional/10. Exception Handling in Task - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Tasks Synchronization - Notes.md'; New = 'Sec 4/notes-kid/11. Tasks Synchronization - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Tasks Synchronization - Notes.md'; New = 'Sec 4/notes-professional/11. Tasks Synchronization - Notes.md' },

    @{ Old = 'Sec 4/notes-kid/Task Cancellation - Notes.md'; New = 'Sec 4/notes-kid/12. Task Cancellation - Notes.md' },
    @{ Old = 'Sec 4/notes-professional/Task Cancellation - Notes.md'; New = 'Sec 4/notes-professional/12. Task Cancellation - Notes.md' }
)

Write-Host 'Renaming Sec 1...'
Rename-NotesFiles -Mappings $sec1

Write-Host 'Renaming Sec 2...'
Rename-NotesFiles -Mappings $sec2

Write-Host 'Renaming Sec 3...'
Rename-NotesFiles -Mappings $sec3

Write-Host 'Renaming Sec 4...'
Rename-NotesFiles -Mappings $sec4

Write-Host 'Done.'

