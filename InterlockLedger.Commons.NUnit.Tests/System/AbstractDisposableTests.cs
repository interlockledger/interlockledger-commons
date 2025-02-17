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

[TestFixture]
public class AbstractDisposableTests
{
    private ConcreteDisposable _disposable;

    [SetUp]
    public void SetUp() => _disposable = new ConcreteDisposable();

    [TearDown]
    public void TearDown() => _disposable?.Dispose();

    [Test]
    public void Dispose_DisposesManagedAndUnmanagedResources() {
        _disposable.Dispose();

        Assert.That(_disposable.ManagedResourcesDisposed, Is.True);
        Assert.That(_disposable.UnmanagedResourcesDisposed, Is.True);
        Assert.That(_disposable.Disposed, Is.True);
    }

    [Test]
    public void Do_ExecutesActionIfNotDisposed() {
        bool actionExecuted = false;
        _ = _disposable.ConcreteDo(() => actionExecuted = true);

        Assert.That(actionExecuted, Is.True);
    }

    [Test]
    public void Do_DoesNotExecuteActionIfDisposed() {
        _disposable.Dispose();
        bool actionExecuted = false;
        _ = _disposable.ConcreteDo(() => actionExecuted = true);

        Assert.That(actionExecuted, Is.False);
    }

    [Test]
    public void Do_ReturnsDefaultIfDisposed() {
        _disposable.Dispose();
        int result = _disposable.ConcreteDo(() => 42, -1);

        Assert.That( result, Is.EqualTo(-1));
    }

    [Test]
    public void UnsafeDo_ReturnsDefaultIfDisposed() {
        _disposable.Dispose();
        string? result = _disposable.ConcreteUnsafeDo(() => "test", "default");
        Assert.That(result, Is.Not.Null);
        Assert.That( result, Is.EqualTo("default"));
    }

    [Test]
    public async Task DoAsync_ExecutesFunctionIfNotDisposed() {
        bool functionExecuted = false;
        await _disposable.ConcreteDoAsync( () => {
            functionExecuted = true;
            return Task.CompletedTask;
        }).ConfigureAwait(false);

        Assert.That(functionExecuted, Is.True);
    }

    [Test]
    public async Task DoAsync_DoesNotExecuteFunctionIfDisposed() {
        _disposable.Dispose();
        bool functionExecuted = false;
        await _disposable.ConcreteDoAsync( () => {
            functionExecuted = true;
            return Task.CompletedTask;
        }).ConfigureAwait(false);

        Assert.That(functionExecuted, Is.False);
    }

    [Test]
    public async Task DoAsync_ReturnsDefaultIfDisposed() {
        _disposable.Dispose();
        int result = await _disposable.ConcreteDoAsync( () => Task.FromResult(42), -1).ConfigureAwait(false);

        Assert.That( result, Is.EqualTo(-1));
    }

    [Test]
    public async Task UnsafeDoAsync_ReturnsDefaultIfDisposed() {
        _disposable.Dispose();
        string? result = await _disposable.ConcreteUnsafeDoAsync(() => Task.FromResult<string?>("test"), "default").ConfigureAwait(false);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo("default"));
    }
}
