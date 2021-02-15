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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static uint AsUint(this string s, uint @default = 0u) => uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : @default;

        public static ulong AsUlong(this string s) => ulong.TryParse(s?.Trim(), out var value) ? value : 0ul;

        public static string Capitalize(this string value) => value.RegexReplace(@"(^[^_]|_+\w)", ToUpperInvariant);

        public static bool IsEmptyOrMatches(this string s, params string[] matches) => string.IsNullOrWhiteSpace(s) || matches.Any(m => s.Trim().Equals(m, StringComparison.InvariantCultureIgnoreCase));

        public static string NoUnderdashes(this string value) => value?.Replace("_", "", StringComparison.OrdinalIgnoreCase);

        public static string Parenthesize(this string s) => s.ParenthesizeIf(!s.Parenthesized());

        public static bool Parenthesized(this string s) {
            if (string.IsNullOrWhiteSpace(s))
                return false;
            s = s.Trim();
            var count = 0;
            if (!s.StartsWith("(", StringComparison.Ordinal)) return false;
            for (var i = 0; i < s.Length; i++) {
                var c = s[i];
                if (c == '(') {
                    count++;
                    continue;
                }
                if (c == ')') {
                    count--;
                    if (count == 0 && (i + 1) < s.Length) return false;
                }
            }
            return count == 0;
        }

        public static string ParenthesizeIf(this string s, bool condition = false) => (condition) ? $"({s})" : s;

        public static string PrefixedBy(this string value, string prefix) => string.IsNullOrWhiteSpace(value) ? null : prefix + value;

        public static string RegexReplace(this string value, string pattern, string replacement) => value.SafeTransform(s => Regex.Replace(s, pattern, replacement));

        public static string RegexReplace(this string value, string pattern, MatchEvaluator me) => value.SafeTransform(s => Regex.Replace(s, pattern, me));

        public static string Required(this string value, string name) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Required", name) : value;

        public static string Reversed(this string s) => string.IsNullOrWhiteSpace(s) ? string.Empty : new string(s.ToCharArray().Reverse().ToArray());

        public static string Safe(this string s) => s.WithDefault(string.Empty);

        public static bool SafeEqualsTo(this string s, string other) => s is null ? other is null : s.Equals(other, StringComparison.InvariantCulture);

        public static string SafeTransform(this string value, Func<string, string> transform) => transform is null ? value : value is null ? null : transform(value);

        public static T SafeTransformTo<T>(this string value, Func<string, T> transform)
            => transform is null || value is null ? default : transform(value);

        public static bool SafeTrimmedEqualsTo(this string s, string other) => SafeEqualsTo(s.TrimToNull(), other.TrimToNull());

        public static string SimplifyAsFileName(this string name)
            => string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException($"Name is empty or null")
                : _nameFilter.Replace(name.Trim(), ".").ToLowerInvariant();

        public static string SingleUnderdashes(this string value) => value.RegexReplace("__+", "_").RegexReplace("^_", "");

        public static IEnumerable<uint> SplitAsUints(this string value, char splitChar)
            => value.Safe().Trim().Split(splitChar).Select(s => s.AsUint());

        public static TEnum ToEnum<TEnum>(this string value, TEnum @default = default) where TEnum : struct
            => value.TrimToNull() is null || !Enum.TryParse<TEnum>(value, ignoreCase: true, out var c) ? @default : c;

        public static string TrimToNull(this string s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        public static byte[] UTF8Bytes(this string s) => Encoding.UTF8.GetBytes(s.WithDefault(string.Empty));

        public static string ValidateNonEmpty(this string parameter, string parameterName)
            => string.IsNullOrWhiteSpace(parameter) ? throw new ArgumentNullException(parameterName) : parameter.Trim();

        public static string WithDefault(this string s, string @default) => string.IsNullOrWhiteSpace(s) ? @default : s.Trim();

        public static string WithDefault(this string s, Func<string> resolver)
            => resolver is null
                ? throw new ArgumentNullException(nameof(resolver))
                : string.IsNullOrWhiteSpace(s) ? resolver() : s.Trim();

        public static string WithNumericSuffix(this string value, uint suffix, char separator = '#')
            => value.SafeTransformTo(s
                => $"{s.Trim().TrimEnd(separator, '0', '1', '2', '3', '4', '5', '6', '7', '8', '9')}{separator}{suffix:0}");

        public static string WithoutWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s) ? string.Empty : Regex.Replace(s, @"[\r\n\s]+", " ").Trim();

        private static readonly Regex _nameFilter = new Regex(@"[\.\s\r\n<>\:""/\\|\?\*]+");

        private static string ToUpperInvariant(Match match) => match.Value.ToUpperInvariant();
    }
}