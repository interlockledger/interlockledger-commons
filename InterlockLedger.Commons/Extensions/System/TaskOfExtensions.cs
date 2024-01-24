// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2023 InterlockLedger Network
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES, LOSS OF USE, DATA, OR PROFITS, OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ******************************************************************************************************************************

using System.Collections.Concurrent;
using System.Diagnostics;

namespace System;
public static class TaskOfExtensions
{
    public static T WaitResult<T>(this Task<T> task) => task.GetAwaiter().GetResult();
    public static void WaitResult(this Task task) => task.GetAwaiter().GetResult();

    private static ConcurrentDictionary<string, ThreadWithQueue>? _threads = new();

    public static void RunOnThread(this Task task, string name, CancellationToken token, bool threadReuse = false) =>
        RunOnThread(task, name, () => DoneWith(name), token, threadReuse);

    public static void RunOnThread(this Task task, string name, Action done, CancellationToken token, bool threadReuse = false) {
        if (token.IsCancellationRequested)
            return;
        _threads?.AddOrUpdate(name.Required(),
                              addValueFactory: _ => new ThreadWithQueue(name, removeByName),
                              updateValueFactory: (_, twq) => twq)
                 .Enqueue(task, done, threadReuse, token);
    }

    private static void removeByName(string? name) {
        if (!name.IsBlank())
            _ = (_threads?.TryRemove(name, out _));
    }
    private static void DoneWith(string name) =>
        Debug.Print($"Done '{name}'");

    public static void StopAllRunOnThreads() {
        if (_threads is not null) {
            foreach (var thread in _threads.Values) {
                Debug.Print($"Disposing of thread '{thread.Name}'");
                thread.Dispose();
            }
        }
        _threads = null;
    }
}

internal sealed class ThreadWithQueue : AbstractDisposable
{
    private sealed record EnqueuedTask(Task task, Action done, CancellationToken token)
    {
        public void Run() {
            try {
                if (!token.IsCancellationRequested && !task.IsCompleted)
                    task.Wait(token);
            } catch (OperationCanceledException) {
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            } finally {
                done();
            }
        }
    }

    private ConcurrentQueue<EnqueuedTask>? _tasks;
    private Thread? _thread;
    private Action<string?>? _removeFromCollection;

    public string? Name => _thread?.Name;

    public ThreadWithQueue(string name, Action<string?> removeFromCollection) {
        _removeFromCollection = removeFromCollection.Required();
        _tasks = new();
        _thread = new Thread(Loop) {
            Name = name.Required(),
            Priority = ThreadPriority.Normal
        };
        _thread.Start();
    }

    private void Loop() {
        while (_tasks is not null) {
            if (_tasks.TryDequeue(out var task)) {
                task.Run();
            } else {
                Thread.Sleep(1000);
            }
            _ = Thread.Yield();
        }
    }

    public void Enqueue(Task task, Action done, bool threadReuse, CancellationToken token) =>
        _tasks?.Enqueue(new EnqueuedTask(task, Done(done, threadReuse), token));

    private Action Done(Action done, bool threadReuse) =>
        () => {
            done();
            if (!threadReuse)
                Dispose();
        };

    protected override void DisposeManagedResources() {
        _tasks = null;
        _removeFromCollection?.Invoke(Name);
        _thread = null;
        _removeFromCollection = null;
    }
}

