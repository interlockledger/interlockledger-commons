// ******************************************************************************************************************************
//
// Copyright (c) 2018-2021 InterlockLedger Network
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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;

namespace System
{
    [TypeConverter(typeof(LimitedRangeConverter))]
    //[JsonConverter(typeof(JsonCustomConverter<LimitedRange>))]
    public struct LimitedRange : IEquatable<LimitedRange>//, IJsonCustom<LimitedRange>
    {
        public readonly ulong End;
        public readonly ulong Start;

        public LimitedRange(ulong start) : this(start, 1) {
        }

        public LimitedRange(ulong start, ushort count) {
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            Start = start;
            checked {
                End = start + count - 1;
            }
        }

        public ushort Count => (ushort)(End - Start + 1);
        public string TextualRepresentation => ToString();

        public static bool operator !=(LimitedRange left, LimitedRange right) => !(left == right);

        public static bool operator ==(LimitedRange left, LimitedRange right) => left.Equals(right);

        public static LimitedRange Resolve(string text) {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Must come one or two numbers separated by '-'", nameof(text));
            var parts = text.Trim('[', ']').Split('-');
            var start = ulong.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
            if (parts.Length == 1)
                return new LimitedRange(start);
            var end = ulong.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
            var range = new LimitedRange(start, end);
            return range;
        }

        public bool Contains(ulong value) => Start <= value && value <= End;

        public bool Contains(LimitedRange other) => Contains(other.Start) && Contains(other.End);

        public override bool Equals(object obj) => obj is LimitedRange limitedRange && Equals(limitedRange);

        public bool Equals(LimitedRange other) => End == other.End && Start == other.Start;

        public override int GetHashCode() => HashCode.Combine(End, Start);

        public bool OverlapsWith(LimitedRange other) => Contains(other.Start) || Contains(other.End) || other.Contains(Start);

        public override string ToString() => $"[{Start}{(End != Start ? "-" + End : "")}]";

        private LimitedRange(ulong start, ulong end) {
            if (end < start)
                throw new ArgumentOutOfRangeException(nameof(end));
            Start = start;
            End = end;
        }
    }

    public static class LimitedRangeExtensions
    {
        public static bool AnyOverlapsWith(this IEnumerable<LimitedRange> first, IEnumerable<LimitedRange> second) => first.Any(f => second.Any(s => s.OverlapsWith(f)));

        public static bool Includes(this IEnumerable<LimitedRange> ranges, ulong value) => ranges.Any(r => r.Contains(value));

        public static bool IsSupersetOf(this IEnumerable<LimitedRange> first, IEnumerable<LimitedRange> second) => second.All(r => first.Any(Value => Value.Contains(r)));
    }

    public class LimitedRangeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            => value is string text
                ? LimitedRange.Resolve(text.Trim())
                : base.ConvertFrom(context, culture, value);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => destinationType == typeof(string) && value is LimitedRange
                ? value.ToString()
                : throw new InvalidOperationException("Can only convert Range to string!!!");
    }
}