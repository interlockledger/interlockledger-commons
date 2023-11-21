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

public interface IResult
{
    const int _noError = 0;

    bool Success { get; }
}

public class Result : IResult
{
    public static Result Ok { get; } = new(IResult._noError);

    public static implicit operator Result(Exception error) => new Error(error);
    public static implicit operator bool(Result result) => result.Required().Success;

    public bool Success => _errorType == IResult._noError;
    protected Result(int errorType) => _errorType = errorType;

    protected readonly int _errorType;
}

public interface IError
{
    const int DefaultErrorType = 500;

    string ErrorMessage { get; }
    int ErrorType { get; }
    Exception? Exception { get; }
}

public sealed class Error : Result, IError
{
    public int ErrorType => _errorType;
    public string ErrorMessage { get; }
    public Exception? Exception { get; }
    public Error(Exception error, int errorType = IError.DefaultErrorType) : base(errorType) {
        Exception = error;
        ErrorMessage = Exception?.Message ?? string.Empty;
    }

    public Error(string errorMessage, int errorType = IError.DefaultErrorType) : base(errorType) =>
        ErrorMessage = errorMessage;
}

public class Result<T> : Result
{
    public static implicit operator T?(Result<T> r) => r.Required().Value;

    public static implicit operator Result<T>(T? value) => new(value);

    public static implicit operator Result<T>(Exception exception) => new Error<T>(exception);
    public static Result<T> Default { get; } = new Result<T>(default(T));
    public T? Value => Success ? _value : throw new InvalidOperationException($"Operation for Result<{typeof(T)}> was not successful.");
    public bool IsDefault => Success && _value is null;

    public Result(T? value) : base(IResult._noError) => _value = value;
    protected Result(int errorType) : base(errorType) { }

    private readonly T? _value;
}

public sealed class Error<T> : Result<T>, IError
{
    public int ErrorType => _errorType;
    public string ErrorMessage { get; }
    public Exception? Exception { get; }
    public Error(Exception error, int errorType = IError.DefaultErrorType) : base(errorType) {
        Exception = error;
        ErrorMessage = Exception?.Message ?? string.Empty;
    }
    public Error(string? errorMessage, int errorType = IError.DefaultErrorType) : base(errorType) =>
        ErrorMessage = errorMessage ?? string.Empty;

    public static explicit operator Error<T>(Error error) =>
        new(error.Required().Exception, error.ErrorMessage, error.ErrorType);

    private Error(Exception? error, string? errorMessage = null, int errorType = IError.DefaultErrorType) : base(errorType) {
        Exception = error;
        ErrorMessage = Exception?.Message ?? errorMessage ?? string.Empty;
    }
}
