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

#nullable enable

namespace System;
[TestFixture]
public class LimitedRangeTests
{

    [Test]
#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument
    public void Equality() {
        Assert.That(LimitedRange.Empty, Is.EqualTo(LimitedRange.Empty));
        Assert.Multiple(() => {
            Assert.That(new LimitedRange(1, 10), Is.EqualTo(new LimitedRange(1, 10)));
            Assert.That(new LimitedRange(1, 10), Is.Not.EqualTo(LimitedRange.Empty));
            Assert.That(LimitedRange.Empty, Is.Not.EqualTo(new LimitedRange(1, 10)));
            Assert.That(new LimitedRange(1, 11), Is.Not.EqualTo(new LimitedRange(1, 10)));
            Assert.That(new LimitedRange(1, 10), Is.Not.EqualTo(new LimitedRange(1, 11)));
        });
    }
#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

    [TestCase("\"[]\"", false, true, "")]
    [TestCase("\"[1]\"", false, false, "")]
    [TestCase("\"[1-10]\"", false, false, "")]
    [TestCase("\"[10-9]\"", true, false, "End of range (9) must be greater than the start (10)")]
    [TestCase("\"[*]\"", true, false, """Input '[*]' does not match ^\[\d+(-\d+)?\]$""")]
    [TestCase("\"[1-70000]\"", true, false, "Range is too wide (Count 70000 > 65535)")]
    public void DeserializeFromJson(string json, bool isInvalid, bool isEmpty, string? cause) {
        var lr = JsonSerializer.Deserialize<LimitedRange>(json);
        AssertLimitedRange(lr, json.Replace("\"", ""), isInvalid, isEmpty, cause);
    }

    //[TestCase("[]", false, true, "")]
    //[TestCase("[1]", false, false, "")]
    //[TestCase("[1-10]", false, false, "")]
    //[TestCase("[10-9]", true, false, "End of range (9) must be greater than the start (10)")]
    //[TestCase("[*]", true, false, """Input '[*]' does not match ^\[\d+(-\d+)?\]$""")]
    //[TestCase("1-10", true, false, """Input '1-10' does not match ^\[\d+(-\d+)?\]$""")]
    //[TestCase("[1-70000]", true, false, "Range is too wide (Count 70000 > 65535)")]
    //public void Parse(string text, bool isInvalid, bool isEmpty, string? cause) {
    //    var lr = LimitedRange.Parse(text);
    //    AssertLimitedRange(lr, text, isInvalid, isEmpty, cause);
    //}

    [TestCase("[]", false, true, "")]
    [TestCase("[1]", false, false, "")]
    [TestCase("[1-10]", false, false, "")]
    [TestCase("[10-9]", true, false, "End of range (9) must be greater than the start (10)")]
    [TestCase("[*]", true, false, """Input '[*]' does not match ^\[\d+(-\d+)?\]$""")]
    [TestCase("1-10", false, false, "")]
    [TestCase("[1-70000]", true, false, "Range is too wide (Count 70000 > 65535)")]
    public void ParseWithProvider(string text, bool isInvalid, bool isEmpty, string? cause) {
        var lr = LimitedRange.Parse(text, provider: null);
        AssertLimitedRange(lr, text, isInvalid, isEmpty, cause, text[0] != '[');
    }

    [Test]
    public void MemberEmpty() => AssertLimitedRange(LimitedRange.Empty, LimitedRange.Empty.TextualRepresentation, false, true);

    [Test]
    public void OneToTen() => AssertLimitedRange(new LimitedRange(1, 10), "[1-10]", isInvalid: false, isEmpty: false);

    [Test]
    public void One() => AssertLimitedRange(new LimitedRange(1), "[1]", isInvalid: false, isEmpty: false);

    [Test]
    public void WrapAround() => AssertLimitedRange(new LimitedRange(ulong.MaxValue, 2),
                                                   "",
                                                   isInvalid: true,
                                                   isEmpty: false,
                                                   cause: "Arithmetic operation resulted in an overflow");


    private static void AssertLimitedRange(LimitedRange lr, string text, bool isInvalid, bool isEmpty, string? cause = null, bool unwrapped = false) {
        Assert.Multiple(() => {
            Assert.That(lr.IsInvalid(), Is.EqualTo(isInvalid), nameof(isInvalid));
            Assert.That(lr.IsEmpty, Is.EqualTo(isEmpty), nameof(isEmpty));
        });
        if (!lr.IsInvalid() && !unwrapped) {
            Assert.That(lr.TextualRepresentation, Is.EqualTo(text));
            string lrAsString = lr; // implicit string conversion
            Assert.That(lrAsString, Is.EqualTo(text));
        } else if (!cause.IsBlank())
            Assert.That(lr.InvalidityCause, Is.EqualTo(cause).IgnoreCase);
        TestContext.WriteLine(lr.ToString());
    }


}
