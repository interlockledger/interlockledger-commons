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

namespace System.Collections.Generic.Tests;

[TestFixture]
public class SingleEnumerableTests
{
    [Test]
    public void SingleEnumerableTest() {
        var single = new SingleEnumerable<int>(42);
        Assert.That(single, Is.Not.Null);
        Assert.Multiple(() => {
            Assert.That(single.First(), Is.EqualTo(42));
            Assert.That(single.Last(), Is.EqualTo(42));
            Assert.That(single.Count(), Is.EqualTo(1));
        });
        var enumerator = single.GetEnumerator();
        Assert.That(enumerator, Is.Not.Null);
        Assert.That(enumerator, Is.InstanceOf<IEnumerator<int>>());
        Assert.Multiple(() => {
            Assert.That(enumerator.Current, Is.EqualTo(0));
            Assert.That(enumerator.MoveNext());
        });
        Assert.Multiple(() => {
            Assert.That(enumerator.Current, Is.EqualTo(42));
            Assert.That(enumerator.MoveNext(), Is.False);
        });
        Assert.That(enumerator.Current, Is.EqualTo(0));
        enumerator.Reset();
        Assert.Multiple(() => {
            Assert.That(enumerator.Current, Is.EqualTo(0));
            Assert.That(enumerator.MoveNext());
        });
        Assert.Multiple(() => {
            Assert.That(enumerator.Current, Is.EqualTo(42));
            Assert.That(enumerator.MoveNext(), Is.False);
        });
        Assert.That(enumerator.Current, Is.EqualTo(0));
    }
}
