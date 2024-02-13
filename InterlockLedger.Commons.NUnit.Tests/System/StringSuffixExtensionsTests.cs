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
public class StringSuffixExtensionsTests
{
    [Test]
    public void WithSuffix() =>
        Assert.Multiple(() => {
            Assert.That(((string?)null).WithSuffix(".json"), Is.Null);
            Assert.That("".WithSuffix(".json"), Is.EqualTo(".json"));
            Assert.That("a".WithSuffix(".json"), Is.EqualTo("a.json"));
            Assert.That("b.JSON".WithSuffix(".json"), Is.EqualTo("b.JSON"));
            Assert.That("c".WithSuffix(null), Is.EqualTo("c"));
            Assert.That("c".WithSuffix(""), Is.EqualTo("c"));
            Assert.That("c".WithSuffix("     "), Is.EqualTo("c"));
            Assert.That("d".WithSuffix("KKK", '!'), Is.EqualTo("d!KKK"));
            Assert.That("d".WithSuffix("!KKK", '!'), Is.EqualTo("d!KKK"));
            Assert.That("d".WithSuffix("!!KKK!", '!'), Is.EqualTo("d!KKK!"));
            Assert.That("e# ".WithSuffix("001", '#'), Is.EqualTo("e#001"));
            Assert.That("file ".WithSuffix("txt"), Is.EqualTo("file.txt"));
            Assert.That(".file ".WithSuffix("txt", '.'), Is.EqualTo(".file.txt"));
            Assert.That("file. ".WithSuffix("txt"), Is.EqualTo("file.txt"));
        });

    [Test]
    public void WithSuffixReplaced() =>
        Assert.Multiple(() => {
            Assert.That(((string?)null).WithSuffixReplaced(".jsonc"), Is.Null);
            Assert.That("".WithSuffixReplaced(".jsonc"), Is.EqualTo(".jsonc"));
            Assert.That("a.json".WithSuffixReplaced(".jsonc"), Is.EqualTo("a.jsonc"));
            Assert.That("b.JSONC".WithSuffixReplaced(".jsonc"), Is.EqualTo("b.jsonc"));
            Assert.That("c.json".WithSuffixReplaced(null), Is.EqualTo("c.json"));
            Assert.That("c.json".WithSuffixReplaced(""), Is.EqualTo("c.json"));
            Assert.That("c.json".WithSuffixReplaced("     "), Is.EqualTo("c.json"));
            Assert.That("d!a".WithSuffixReplaced("KKK", '!'), Is.EqualTo("d!KKK"));
            Assert.That("d!a".WithSuffixReplaced("!KKK", '!'), Is.EqualTo("d!KKK"));
            Assert.That("d!a".WithSuffixReplaced("!!KKK!", '!'), Is.EqualTo("d!KKK!"));
            Assert.That("e# ".WithSuffixReplaced("001", '#'), Is.EqualTo("e#001"));
            Assert.That("file.doc ".WithSuffixReplaced("txt"), Is.EqualTo("file.txt"));
            Assert.That(".file.doc ".WithSuffixReplaced("txt", '.'), Is.EqualTo(".file.txt"));
            Assert.That("file. ".WithSuffixReplaced("txt"), Is.EqualTo("file.txt"));
        });
}
