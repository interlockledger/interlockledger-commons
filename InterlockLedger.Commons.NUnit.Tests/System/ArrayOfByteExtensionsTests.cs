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
public class ArrayOfByteExtensionsTests
{
    [Test]
    public void Append() {
        byte[] bytes = [1, 2, 3];
        byte[] newBytes = [4, 5, 6];
        byte[]? nullBytes = null;
        byte[]? nullNewBytes = null;
        byte[]? appended = bytes.Append(newBytes);
        Assert.That(appended, Is.EqualTo(new byte[] { 1, 2, 3, 4, 5, 6 }));
        appended = bytes.Append(nullNewBytes!);
        Assert.That(appended, Is.EqualTo(bytes));
        appended = nullBytes.Append(newBytes);
        Assert.That(appended, Is.EqualTo(newBytes));
        appended = nullBytes.Append(nullNewBytes);
        Assert.That(appended, Is.Null);
    }

    [Test]
    public void AsLiteral() {
        byte[] bytes = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        using (Assert.EnterMultipleScope()) {
            Assert.That(bytes.AsLiteral(5), Is.EqualTo("new byte[] { 1, 2, 3, 4, 5 ...}"));
            Assert.That(bytes.AsLiteral(), Is.EqualTo("new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }"));
        }

    }

    [Test]
    public void AsLong() {
        byte[] bytes = [0, 0, 0, 0, 0, 0, 0, 1];
        Assert.That(bytes.AsLong(), Is.EqualTo(1L));
    }

    [Test]
    public void AsULong() {
        byte[] bytes = [0, 0, 0, 0, 0, 0, 0, 1];
        Assert.That(bytes.AsULong(), Is.EqualTo(1UL));
    }

    [Test]
    public void AsUTF8String() {
        byte[] bytes = "Test"u8.ToArray();
        Assert.That(bytes.AsUTF8String(), Is.EqualTo("Test"));
    }

    [Test]
    public void CompareTo() {
        byte[] bytes1 = [1, 2, 3];
        byte[] bytes2 = [1, 2, 4];
        byte[] bytes3 = [1, 2, 3, 4];
        using (Assert.EnterMultipleScope()) {
            Assert.That(bytes1.CompareTo(bytes2), Is.EqualTo(-1));
            Assert.That(bytes2.CompareTo(bytes1), Is.EqualTo(1));
            Assert.That(bytes1.CompareTo(bytes1), Is.Zero);
            Assert.That(bytes1.CompareTo(bytes3), Is.EqualTo(-1));
            Assert.That(bytes3.CompareTo(bytes1), Is.EqualTo(1));
        }

    }

    [Test]
    public void FromSafeBase64() {
        string base64 = "AQID";
        byte[] bytes = base64.FromSafeBase64();
        Assert.That(bytes, Is.EqualTo(new byte[] { 1, 2, 3 }));
    }

    [Test]
    public void HasSameBytesAs() {
        byte[] bytes1 = [1, 2, 3];
        byte[] bytes2 = [1, 2, 3];
        byte[] bytes3 = [1, 2, 4];
        using (Assert.EnterMultipleScope()) {
            Assert.That(bytes1.HasSameBytesAs(bytes2), Is.True);
            Assert.That(bytes1.HasSameBytesAs(bytes3), Is.False);
        }

    }

    [Test]
    public void PartOf() {
        byte[] bytes = [1, 2, 3, 4, 5];
        byte[] part = bytes.PartOf(3, 1);
        Assert.That(part, Is.EqualTo(new byte[] { 2, 3, 4 }));
    }

    [Test]
    public void ToSafeBase64() {
        byte[] bytes = [1, 2, 3];
        string base64 = bytes.ToSafeBase64();
        Assert.That(base64, Is.EqualTo("AQID"));
    }

    [Test]
    public void SafeLength() {
        byte[]? bytes = null;
        Assert.That(bytes.SafeLength(), Is.Zero);
        bytes = [];
        Assert.That(bytes.SafeLength(), Is.Zero);
        bytes = [1, 2, 3];
        Assert.That(bytes.SafeLength(), Is.EqualTo(3));
    }
}