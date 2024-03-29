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

namespace System.Buffers;

public static class ReadOnlySequenceExtensions
{
    public static ReadOnlySequence<byte> Add(this ReadOnlySequence<byte> sequence, ReadOnlySequence<byte> otherSequence) =>
         sequence.JoinTo(otherSequence).ToSequence();

    public static ReadOnlySequence<byte> Add(this ReadOnlySequence<byte> sequence, ReadOnlyMemory<byte> memory) =>
         sequence.Append(memory).ToSequence();

    public static ReadOnlySequence<byte> Add(this ReadOnlySequence<byte> sequence, byte[] array) =>
         sequence.Add(new ReadOnlyMemory<byte>(array));

    public static ReadOnlySequence<byte> Add(this ReadOnlySequence<byte> sequence, byte[] array, int start, int length) =>
         sequence.Add(new ReadOnlyMemory<byte>(array, start, length));

    public static ReadOnlySequenceStream AsStream(this ReadOnlySequence<byte> memory) => new(memory);

    public static T AsStreamDo<T>(this ReadOnlySequence<byte> memory, Func<Stream, T> func) {
        ArgumentNullException.ThrowIfNull(func);
        using Stream s = memory.AsStream();
        return func(s);
    }

    public static ReadOnlySequence<byte> Prepend(this ReadOnlySequence<byte> sequence, ReadOnlySequence<byte> otherSequence) =>
         otherSequence.JoinTo(sequence).ToSequence();

    public static ReadOnlySequence<byte> Prepend(this ReadOnlySequence<byte> sequence, ReadOnlyMemory<byte> memory) =>
         sequence.PrependMemory(memory).ToSequence();

    public static ReadOnlySequence<byte> Prepend(this ReadOnlySequence<byte> sequence, byte[] array) =>
         sequence.Prepend(new ReadOnlyMemory<byte>(array));

    public static ReadOnlySequence<byte> Prepend(this ReadOnlySequence<byte> sequence, byte[] array, int start, int length) =>
         sequence.Prepend(new ReadOnlyMemory<byte>(array, start, length));

    public static ReadOnlySequence<byte> Realloc(this ReadOnlySequence<byte> body) {
        byte[] newBuffer = new byte[body.Length];
        body.CopyTo(newBuffer.AsSpan());
        return new ReadOnlySequence<byte>(newBuffer);
    }

#pragma warning disable CA1055 // URI-like return values should not be strings
    public static string ToUrlSafeBase64(this ReadOnlySequence<byte> readOnlyBytes) =>
         readOnlyBytes.Length > 256
            ? ReadOnlyMemoryExtensions.ToUrlSafeBase64(readOnlyBytes.Slice(0, 256).ToArray()) + "..."
            : ReadOnlyMemoryExtensions.ToUrlSafeBase64(readOnlyBytes.ToArray());
#pragma warning restore CA1055 // URI-like return values should not be strings

    private static IEnumerable<ReadOnlyMemory<byte>> Append(this ReadOnlySequence<byte> sequence, ReadOnlyMemory<byte> memory) {
        var current = sequence.Start;
        while (sequence.TryGet(ref current, out var segment))
            yield return segment;
        yield return memory;
    }

    private static IEnumerable<ReadOnlyMemory<byte>> JoinTo(this ReadOnlySequence<byte> sequence, ReadOnlySequence<byte> otherSequence) {
        var current = sequence.Start;
        while (sequence.TryGet(ref current, out var segment))
            yield return segment;
        current = otherSequence.Start;
        while (otherSequence.TryGet(ref current, out var segment))
            yield return segment;
    }

    private static IEnumerable<ReadOnlyMemory<byte>> PrependMemory(this ReadOnlySequence<byte> sequence, ReadOnlyMemory<byte> memory) {
        yield return memory;
        var current = sequence.Start;
        while (sequence.TryGet(ref current, out var segment))
            yield return segment;
    }
}
