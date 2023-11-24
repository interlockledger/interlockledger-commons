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

using System.Runtime.InteropServices;

namespace System.Buffers;

public static class ReadOnlyMemoryExtensions
{
    public static ArraySegment<byte> ToArraySegment(this ReadOnlyMemory<byte> memory) =>
         !MemoryMarshal.TryGetArray(memory, out var result)
            ? throw new InvalidOperationException("Buffer backed by array was expected")
            : result;

    public static ReadOnlySequence<byte> ToSequence(this IEnumerable<ReadOnlyMemory<byte>> segments) =>
         (segments?.Count() ?? 0) switch {
             0 => ReadOnlySequence<byte>.Empty,
             1 => new ReadOnlySequence<byte>(segments!.First()),
             _ => LinkedSegment.Link(segments!)
         };

#pragma warning disable CA1055 // URI-like return values should not be strings
    public static string ToUrlSafeBase64(this byte[] bytes) =>
         Convert.ToBase64String(bytes ?? throw new ArgumentNullException(nameof(bytes)))
           .Trim('=')
           .Replace('+', '-')
           .Replace('/', '_');
#pragma warning restore CA1055 // URI-like return values should not be strings

    private sealed class LinkedSegment : ReadOnlySequenceSegment<byte>
    {
        public int Length => Memory.Length;

        public long NextRunningIndex => RunningIndex + Length;

        public static ReadOnlySequence<byte> Link(IEnumerable<ReadOnlyMemory<byte>> segments) {
            LinkedSegment? first = null;
            LinkedSegment? current = null;
            foreach (var segment in segments) {
                var next = new LinkedSegment(segment, current?.NextRunningIndex ?? 0);
                first ??= next;
                if (current is not null)
                    current.Next = next;
                current = next;
            }

            return new ReadOnlySequence<byte>(first.Required(), 0, current.Required(), current.Length);
        }

        private LinkedSegment(ReadOnlyMemory<byte> memory, long runningIndex) {
            Memory = memory;
            RunningIndex = runningIndex;
        }
    }
}
