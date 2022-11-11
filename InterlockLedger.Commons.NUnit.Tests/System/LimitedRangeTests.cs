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


namespace System;

[TestFixture]
public class LimitedRangeTests
{
    [TestCase("\"[]\"", false, true, "")]
    [TestCase("\"[1]\"", false, false, "")]
    [TestCase("\"[1-10]\"", false, false, "")]
    [TestCase("\"[10-9]\"", true, false, "End of range [9] must be greater than the start [10]")]
    [TestCase("\"[*]\"", true, false, "Bad Format")]
    [TestCase("\"[1-70000]\"", true, false, "Range is too wide (Count [70000] > 65535)")]
    public void DeserializeFromJson(string json, bool isInvalid, bool isEmpty, string? cause) {
        var lr = JsonSerializer.Deserialize<LimitedRange>(json);
        AssertLimitedRange(lr, json.Replace("\"", ""), isInvalid, isEmpty, cause);
    }

    [TestCase("[]", false, true, "")]
    [TestCase("[1]", false, false, "")]
    [TestCase("[1-10]", false, false, "")]
    [TestCase("[10-9]", true, false, "End of range [9] must be greater than the start [10]")]
    [TestCase("[*]", true, false, "Bad Format")]
    [TestCase("[1-70000]", true, false, "Range is too wide (Count [70000] > 65535)")]
    public void Resolve(string text, bool isInvalid, bool isEmpty, string? cause) {
        var lr = ITextual<LimitedRange>.Resolve(text);
        AssertLimitedRange(lr, text, isInvalid, isEmpty, cause);
    }

    [TestCase("[]", false, true, "")]
    [TestCase("[1]", false, false, "")]
    [TestCase("[1-10]", false, false, "")]
    [TestCase("[10-9]", true, false, "End of range [9] must be greater than the start [10]")]
    [TestCase("[*]", true, false, "Bad Format: '[*]'")]
    [TestCase("[1-70000]", true, false, "Range is too wide (Count [70000] > 65535)")]
    public void FromString(string text, bool isInvalid, bool isEmpty, string? cause) {
        var lr = LimitedRange.FromString(text);
        AssertLimitedRange(lr, text, isInvalid, isEmpty, cause);
    }

    private static void AssertLimitedRange(LimitedRange lr, string text, bool isInvalid, bool isEmpty, string? cause = null) {
        Assert.AreEqual(isInvalid, lr.IsInvalid, nameof(isInvalid));
        Assert.AreEqual(isEmpty, lr.IsEmpty, nameof(isEmpty));
        if (!lr.IsInvalid) {
            Assert.That(lr.TextualRepresentation, Is.EqualTo(text));
        } else if (!cause.IsBlank())
            StringAssert.AreEqualIgnoringCase(cause, lr.InvalidityCause);
        TestContext.WriteLine(lr.ToString());
    }

    [Test]
    public void MemberEmpty() => AssertLimitedRange(LimitedRange.Empty, LimitedRange.Empty.TextualRepresentation, false, true);

    [Test]
    public void MemberInvalid() => AssertLimitedRange(LimitedRange.Invalid, LimitedRange.Invalid.TextualRepresentation, true, false);

    [Test]
    public void OneToTen() => AssertLimitedRange(new LimitedRange(1, 10), "[1-10]", false, false);

    [Test]
    public void One() => AssertLimitedRange(new LimitedRange(1), "[1]", false, false);

    [Test]
    public void WrapAround() => AssertLimitedRange(new LimitedRange(ulong.MaxValue, 2), "[1-10]", true, false, "Arithmetic operation resulted in an overflow");


}
