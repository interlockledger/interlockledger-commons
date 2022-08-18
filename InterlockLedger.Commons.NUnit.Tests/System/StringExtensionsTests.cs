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

#nullable enable

using NUnit.Framework;

using static System.ObjectExtensions;
using static System.ObjectExtensionsTests;

namespace System;

[TestFixture]
public class StringExtensionsTests
{
    [Test]
    public void RequiredForString() => _ = TestRequired(null, "stringNull") && TestRequired(string.Empty, "stringEmpty");
    [Test]
    public void RequiredUsingForString() => _ = TestRequiredUsing(null, "stringNull") && TestRequiredUsing(string.Empty, "stringEmpty");

    private static bool TestRequired(string? value, string name) =>
        AssertArgumentException<ArgumentException>(name, () => value.Required(name))
        && AssertArgumentException<ArgumentException>(nameof(value), () => value.Required());

    private static bool TestRequiredUsing(string? value, string name) =>
        AssertArgumentException<ArgumentException>(name, () => value.RequiredUsing(n => new ArgumentException(_expectedExceptionMessageStart, n), name))
        && AssertArgumentException<ArgumentException>(name, () => value.RequiredUsing(ArgRequired, name))
        && AssertArgumentException<ArgumentNullException>(name, () => value.RequiredUsing(n => new ArgumentNullException(n, _expectedExceptionMessageStart), name))
        && AssertArgumentException<ArgumentNullException>(name, () => value.RequiredUsing(ArgNullRequired, name))
        && AssertArgumentException<ArgumentException>(nameof(value), () => value.RequiredUsing(n => new ArgumentException(_expectedExceptionMessageStart, n)))
        && AssertArgumentException<ArgumentException>(nameof(value), () => value.RequiredUsing(ArgRequired))
        && AssertArgumentException<ArgumentNullException>(nameof(value), () => value.RequiredUsing(n => new ArgumentNullException(n, _expectedExceptionMessageStart)))
        && AssertArgumentException<ArgumentNullException>(nameof(value), () => value.RequiredUsing(ArgNullRequired));

}