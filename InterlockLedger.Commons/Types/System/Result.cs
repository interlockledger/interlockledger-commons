// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2022 InterlockLedger Network
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

public class Result
{

    public static Result Ok { get; } = new(true, null);
    public static Result WithError(Exception error) => new(false, error);

    public static implicit operator Result(Exception error) => new(false, error);

    public bool Success { get; }
    public string Message => _error?.Message ?? "";
    public Exception Error => _error ?? throw new InvalidOperationException($"Error property for this Result not set.");

    protected Result(bool success, Exception? error) {
        Success = success;
        _error = error;
    }

    protected readonly Exception? _error;
}

public sealed class Result<T> : Result where T : class
{
    public T Value => Success
        ? _payload ?? throw new InvalidOperationException($"Payload for Result<{typeof(T)}> was not set.")
        : throw new InvalidOperationException($"Operation for Result<{typeof(T)}> was not successful.");

    public static Result<T> With(T value) => new(value);
    public static new Result<T> WithError(Exception error) => new(error);

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Exception exception) => new(exception);
    private Result(T? value, bool success, Exception? error) : base(success, error) => _payload = value;

    public Result(T value) : this(value.Required(), true, null) { }

    public Result(Exception error) : base(false, error) {
    }

    private readonly T? _payload;
}
