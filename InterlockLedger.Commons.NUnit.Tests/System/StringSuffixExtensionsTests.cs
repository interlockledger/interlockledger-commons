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

using NUnit.Framework;

namespace System;

[TestFixture]
public class StringSuffixExtensionsTests
{
    [Test]
    public void WithSuffix() {
        Assert.IsNull(((string)null).WithSuffix(".json"));
        Assert.AreEqual(".json", "".WithSuffix(".json"));
        Assert.AreEqual("a.json", "a".WithSuffix(".json"));
        Assert.AreEqual("b.JSON", "b.JSON".WithSuffix(".json"));
        Assert.AreEqual("c", "c".WithSuffix(null));
        Assert.AreEqual("c", "c".WithSuffix(""));
        Assert.AreEqual("c", "c".WithSuffix("     "));
        Assert.AreEqual("d!KKK", "d".WithSuffix("KKK", '!'));
        Assert.AreEqual("d!KKK", "d".WithSuffix("!KKK", '!'));
        Assert.AreEqual("d!KKK!", "d".WithSuffix("!!KKK!", '!'));
        Assert.AreEqual("e#001", "e# ".WithSuffix("001", '#'));
        Assert.AreEqual("file.txt", "file ".WithSuffix("txt"));
        Assert.AreEqual(".file.txt", ".file ".WithSuffix("txt", '.'));
        Assert.AreEqual("file.txt", "file. ".WithSuffix("txt"));
    }

    [Test]
    public void WithSuffixReplaced() {
        Assert.IsNull(((string)null).WithSuffixReplaced(".jsonc"));
        Assert.AreEqual(".jsonc", "".WithSuffixReplaced(".jsonc"));
        Assert.AreEqual("a.jsonc", "a.json".WithSuffixReplaced(".jsonc"));
        Assert.AreEqual("b.jsonc", "b.JSONC".WithSuffixReplaced(".jsonc"));
        Assert.AreEqual("c.json", "c.json".WithSuffixReplaced(null));
        Assert.AreEqual("c.json", "c.json".WithSuffixReplaced(""));
        Assert.AreEqual("c.json", "c.json".WithSuffixReplaced("     "));
        Assert.AreEqual("d!KKK", "d!a".WithSuffixReplaced("KKK", '!'));
        Assert.AreEqual("d!KKK", "d!a".WithSuffixReplaced("!KKK", '!'));
        Assert.AreEqual("d!KKK!", "d!a".WithSuffixReplaced("!!KKK!", '!'));
        Assert.AreEqual("e#001", "e# ".WithSuffixReplaced("001", '#'));
        Assert.AreEqual("file.txt", "file.doc ".WithSuffixReplaced("txt"));
        Assert.AreEqual(".file.txt", ".file.doc ".WithSuffixReplaced("txt", '.'));
        Assert.AreEqual("file.txt", "file. ".WithSuffixReplaced("txt"));
    }
}
