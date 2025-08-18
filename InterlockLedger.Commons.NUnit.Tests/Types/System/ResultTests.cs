// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2025 InterlockLedger Network
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
public class ResultTests
{
    [Test]
    public void ResultOk() {
        var result = Result.Ok;
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void ResultFromException() {
        var exception = new Exception("Test Exception");
        Result result = exception;
        Assert.That(result.Success, Is.False);
        Assert.That(result, Is.TypeOf<Error>());
        var error = (Error)result;
        Assert.That(error.ErrorMessage, Is.EqualTo("Test Exception"));
        Assert.That(error.Exception, Is.EqualTo(exception));
    }

    [Test]
    public void ResultBoolConversion() {
        bool success = Result.Ok;
        Assert.That(success, Is.True);
        Result error = new Exception();
        success = error;
        Assert.That(success, Is.False);
    }

    [Test]
    public void ErrorFromException() {
        var exception = new Exception("Test Exception");
        var error = new Error(exception);
        Assert.That(error.Success, Is.False);
        Assert.That(error.ErrorMessage, Is.EqualTo("Test Exception"));
        Assert.That(error.Exception, Is.EqualTo(exception));
        Assert.That(error.ErrorType, Is.EqualTo(IError.DefaultErrorType));
    }

    [Test]
    public void ErrorFromString() {
        var error = new Error("Test Error");
        Assert.That(error.Success, Is.False);
        Assert.That(error.ErrorMessage, Is.EqualTo("Test Error"));
        Assert.That(error.Exception, Is.Null);
        Assert.That(error.ErrorType, Is.EqualTo(IError.DefaultErrorType));
    }

    [Test]
    public void ResultOfTFromValue() {
        Result<int> result = 123;
        Assert.That(result.Success, Is.True);
        Assert.That(result.Value, Is.EqualTo(123));
    }

    [Test]
    public void ResultOfTFromException() {
        var exception = new Exception("Test Exception");
        Result<int> result = exception;
        Assert.That(result.Success, Is.False);
        Assert.Throws<InvalidOperationException>(() => { var _ = result.Value; });
    }

    [Test]
    public void ResultOfTToT() {
        Result<int> result = 123;
        int value = result;
        Assert.That(value, Is.EqualTo(123));
    }

    [Test]
    public void ErrorOfTFromException() {
        var exception = new Exception("Test Exception");
        var error = new Error<int>(exception);
        Assert.That(error.Success, Is.False);
        Assert.That(error.ErrorMessage, Is.EqualTo("Test Exception"));
        Assert.That(error.Exception, Is.EqualTo(exception));
    }

    [Test]
    public void ResultExtensionsToConvertedResult() {
        Result<int> result = 123;
        Result<string> convertedResult = result.ToConvertedResult(v => v.ToString());
        Assert.That(convertedResult.Success, Is.True);
        Assert.That(convertedResult.Value, Is.EqualTo("123"));

        Result<int> errorResult = new Exception("Error");
        convertedResult = errorResult.ToConvertedResult(v => v.ToString());
        Assert.That(convertedResult.Success, Is.False);
    }
}
