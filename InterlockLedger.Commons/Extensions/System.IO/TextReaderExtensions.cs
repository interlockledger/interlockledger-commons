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

namespace System.IO;
public static class TextReaderExtensions
{
    public static sbyte ReadInt8(this TextReader reader) => ReadNumber(reader, (ulong)sbyte.MaxValue, v => (sbyte)v, true);
    public static short ReadInt16(this TextReader reader) => ReadNumber(reader, (ulong)short.MaxValue, v => (short)v, true);
    public static int ReadInt32(this TextReader reader) => ReadNumber(reader, int.MaxValue, v => (int)v, true);
    public static long ReadInt64(this TextReader reader) => ReadNumber(reader, long.MaxValue, v => (long)v, true);
    public static byte ReadUInt8(this TextReader reader) => ReadNumber(reader, byte.MaxValue, v => (byte)v, false);
    public static ushort ReadUInt16(this TextReader reader) => ReadNumber(reader, ushort.MaxValue, v => (ushort)v, false);
    public static uint ReadUInt32(this TextReader reader) => ReadNumber(reader, uint.MaxValue, v => (uint)v, false);
    public static ulong ReadUInt64(this TextReader reader) => ReadNumber(reader, ulong.MaxValue, v => v, false);
    public static string ReadToken(this TextReader reader) {
        StringBuilder sb = new();
        const string WhatImReading = "token";
        char ch = CheckChar(reader.SkipWhiteSpace(), WhatImReading);
        while (true) {
            _ = sb.Append(ch);
            char? next = reader.ReadChar();
            if (next is null || char.IsWhiteSpace(next.Value))
                break;
            ch = next.Value;
        }
        return sb.ToString();
    }


    public static char? SkipWhiteSpace(this TextReader reader) {
        char? ch;
        do ch = reader.ReadChar();
        while (ch.HasValue && char.IsWhiteSpace(ch.Value));
        return ch;
    }

    public static char? ReadChar(this TextReader reader) {
        int ch = reader.Read();
        return ch >= 0 ? (char?)ch : null;
    }


    private static char CheckChar(char? ch, string what) =>
        ch ?? throw new InvalidOperationException($"Could not find a {what} to read");

    private static T ReadNumber<T>(TextReader reader, ulong maxValue, Func<ulong, T> cast, bool signed) where T : Numerics.INumber<T> {
        const string WhatImReading = "number";
        ulong value = 0;
        bool isNegative = false;
        char ch = CheckChar(reader.SkipWhiteSpace(), WhatImReading);
        if (ch.In('-', '+')) {
            if (signed) {
                isNegative = ch == '-';
                ch = CheckChar(reader.ReadChar(), WhatImReading);
            } else
                throw new InvalidOperationException($"Unexpected sign character '{ch}' in unsigned number");

        }
        while (true) {
            if (!char.IsAsciiDigit(ch)) {
                if (char.IsWhiteSpace(ch))
                    break;
                else
                    throw new InvalidOperationException($"Invalid character '{ch}' in number");
            } else {
                value = value * 10 + (byte)(ch - '0');
                char? digit = reader.ReadChar();
                if (digit is null || char.IsWhiteSpace(digit.Value))
                    break;
                ch = digit.Value;
            }
        }
        if (value > maxValue)
            throw new InvalidOperationException("Number is too big");
        else {
            var castValue = cast(value);
            return isNegative ? -castValue : castValue;
        }
    }
}
