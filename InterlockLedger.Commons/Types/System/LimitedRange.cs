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

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace System;

[TypeConverter(typeof(TypeCustomConverter<LimitedRange>))]
[JsonConverter(typeof(JsonCustomConverter<LimitedRange>))]
public readonly partial struct LimitedRange : ITextual<LimitedRange>
{
    public readonly ulong End;
    public readonly ulong Start;

    public LimitedRange(ulong start) : this(start, 1) {
    }

    public LimitedRange(ulong start, ushort count) {
        if (count != 0) {
            Start = start;
            try {
                checked {
                    End = start + count - 1;
                }
                TextualRepresentation = NormalRepresentation(Start, End);
            } catch (OverflowException) {
                End = start;
                IsInvalid = true;
                TextualRepresentation = $"[{Start}+{count}]";
                InvalidityCause = "Arithmetic operation resulted in an overflow";
            }
        } else {
            End = Start = 0;
            TextualRepresentation = "[]";
            IsEmpty = true;
        }
    }


    public ushort Count => (ushort)(End - Start + 1);
    public bool IsEmpty { get; }
    public bool IsInvalid { get; }
    public string TextualRepresentation { get; }
    public string? InvalidityCause { get; }


    public static bool operator !=(LimitedRange left, LimitedRange right) => !(left == right);

    public static bool operator ==(LimitedRange left, LimitedRange right) => left.Equals(right);

    public bool Contains(ulong value) => Start <= value && value <= End;

    public bool Contains(LimitedRange other) => Contains(other.Start) && Contains(other.End);

    public override bool Equals(object? obj) => obj is LimitedRange limitedRange && Equals(limitedRange);

    public bool Equals(LimitedRange other) => _traits.EqualsForAnyInstances(other);

    public override int GetHashCode() => HashCode.Combine(End, Start);

    public bool OverlapsWith(LimitedRange other) => Contains(other.Start) || Contains(other.End) || other.Contains(Start);

    public override string ToString() => $"{TextualRepresentation}{_traits.InvalidityCoda}";

    public bool EqualsForValidInstances(LimitedRange other) => End == other.End && Start == other.Start;


    public static LimitedRange Empty { get; } = new LimitedRange(0, 0);
    public static LimitedRange InvalidBy(string cause) => new(ulong.MaxValue, cause);

    public static Regex Mask { get; } = MaskRegex();
    public static string MessageForMissing { get; } = "No LimitedRange";
    public static string MessageForInvalid(string? textualRepresentation) => $"Not a valid LimitedRange '{textualRepresentation}'";
    public static LimitedRange Parse(string s, IFormatProvider? provider) => FromString(s);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out LimitedRange result) =>
        ITextual<LimitedRange>.TryParse(s, out result);

    public static LimitedRange FromString(string textualRepresentation) {
        string[] parts = textualRepresentation.Safe().Trim('[', ']').Split('-');
        bool justOnePart = parts.Length == 1;
        if (justOnePart && parts[0].IsBlank())
            return Empty;
        bool noStartValue = !ulong.TryParse(parts[0], CultureInfo.InvariantCulture, out ulong start);
        ulong end = start;
        bool noEndValue = !justOnePart && !ulong.TryParse(parts[1], CultureInfo.InvariantCulture, out end);
        return noStartValue || noEndValue
            ? new LimitedRange(start, ITextual<LimitedRange>.InvalidByNotMatchingMask(textualRepresentation))
            : end >= start && end <= start + ushort.MaxValue
                ? new LimitedRange(start, end, NormalRepresentation(start, end))
                : new LimitedRange(start, InvalidityCauseFrom(start, end));
    }


    [GeneratedRegex("""^\[\d+(-\d+)?\]$""")]
    private static partial Regex MaskRegex();

    private ITextual<LimitedRange> _traits => this;

    private LimitedRange(ulong start, ulong end, string textualRepresentation) {
        Start = start;
        End = end;
        TextualRepresentation = textualRepresentation;
    }

    private LimitedRange(ulong start, string invalidityCause) {
        Start = start;
        End = start;
        TextualRepresentation = "[?]";
        IsInvalid = true;
        InvalidityCause = invalidityCause;
    }

    private static string NormalRepresentation(ulong start, ulong end) =>
        end == start ? $"[{start}]" : $"[{start}-{end}]";
    private static string InvalidityCauseFrom(ulong start, ulong end) =>
        end < start
            ? $"End of range ({end}) must be greater than the start ({start})"
            : $"Range is too wide (Count {end + 1 - start} > {ushort.MaxValue})";
}

public static class IEnumerableOfLimitedRangeExtensions
{
    public static bool AnyOverlapsWith(this IEnumerable<LimitedRange> first, IEnumerable<LimitedRange> second) => first.Any(f => second.Any(s => s.OverlapsWith(f)));

    public static bool Includes(this IEnumerable<LimitedRange> ranges, ulong value) => ranges.Any(r => r.Contains(value));

    public static bool IsSupersetOf(this IEnumerable<LimitedRange> first, IEnumerable<LimitedRange> second) => second.All(r => first.Any(Value => Value.Contains(r)));
}
