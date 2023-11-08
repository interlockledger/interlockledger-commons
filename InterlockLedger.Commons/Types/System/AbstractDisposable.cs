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

namespace System;

public abstract class AbstractDisposable : IDisposable
{
    [JsonIgnore]
    public bool Disposed => _disposed != 0;

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected abstract void DisposeManagedResources();

    protected virtual void DisposeUnmanagedResources() { }

    protected void Do(Action action) {
        if (!Disposed) action.Required()();
    }
    protected T Do<T>(Func<T> function, T @default = default) where T : struct =>
        Disposed ? @default : function.Required()();
    protected T? UnsafeDo<T>(Func<T?> function, T? @default = default) where T : class =>
        Disposed ? @default : function.Required()();
    protected T? UnsafeDo<T>(Func<T?> function) =>
        Disposed ? default : function.Required()();


    protected async Task DoAsync(Func<Task> function) {
        if (!Disposed)
            await function.Required()().ConfigureAwait(false);
    }

    protected async Task<T> DoAsync<T>(Func<Task<T>> function, T @default = default) where T : struct =>
        Disposed ? @default : await function.Required()().ConfigureAwait(false);
    protected async Task<T?> UnsafeDoAsync<T>(Func<Task<T?>> function, T? @default = default) where T : class =>
        Disposed ? @default : await function.Required()().ConfigureAwait(false);
    protected async Task<T?> UnsafeDoAsync<T>(Func<Task<T?>> function) =>
        Disposed ? default : await function.Required()().ConfigureAwait(continueOnCapturedContext: false);


    private volatile int _disposed;

    ~AbstractDisposable() {
        Dispose(false);
    }

    private void Dispose(bool disposing) {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0) {
            if (disposing)
                DisposeManagedResources();
            DisposeUnmanagedResources();
        }
    }
}
