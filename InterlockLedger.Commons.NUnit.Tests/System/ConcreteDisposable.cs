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
public class ConcreteDisposable : AbstractDisposable
{
    public bool ManagedResourcesDisposed { get; private set; }
    public bool UnmanagedResourcesDisposed { get; private set; }
    protected override void DisposeManagedResources() => ManagedResourcesDisposed = true;
    protected override void DisposeUnmanagedResources() => UnmanagedResourcesDisposed = true;
    public Task ConcreteDoAsync(Func<Task> function) => DoAsync(function);
    public T ConcreteDo<T>(Func<T> function, T @default = default) where T : struct => Do(function, @default);
    public T? ConcreteUnsafeDo<T>(Func<T?> function) => UnsafeDo(function);
    public T? ConcreteUnsafeDo<T>(Func<T?> function, T? @default = default) where T : class => UnsafeDo(function, @default);
    public Task<T?> ConcreteUnsafeDoAsync<T>(Func<Task<T?>> function) => UnsafeDoAsync(function);
    public Task<T?> ConcreteUnsafeDoAsync<T>(Func<Task<T?>> function, T? @default = default) where T : class => UnsafeDoAsync(function, @default);
    public Task<T> ConcreteDoAsync<T>(Func<Task<T>> function, T @default = default) where T : struct => DoAsync(function, @default);
    public void ConcreteDo(Action action) => Do(action);
}
