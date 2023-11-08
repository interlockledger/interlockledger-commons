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

using System.Text.Json;

using static System.ObjectExtensions;

namespace System;

public static partial class StringExtensions
{
    public static JsonSerializerOptions DefaultJsonOptions { get; } = new JsonSerializerOptions {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        IgnoreReadOnlyProperties = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    };

    public static string Safe(this string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value;

    public static bool IsNumeric(this string s) =>
        s.All(char.IsDigit);

    public static string LimitedTo(this string? value, short limit) =>
        (value = value.Safe().Trim()).Length < limit
            ? value
            : value[0..limit];

    public static uint AsUint(this string? s, uint @default = 0u) =>
        uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint result) ? result : @default;

    public static ulong AsUlong(this string? s) =>
        ulong.TryParse(s?.Trim(), out ulong value) ? value : 0ul;

    public static string? Capitalize(this string? value) =>
        value.RegexReplace(@"(^[^_]|_+\w)", ToUpperInvariant);

    public static bool IsBlank([NotNullWhen(returnValue: false)] this string? value) =>
        string.IsNullOrWhiteSpace(value);

    public static bool IsEmptyOrMatches(this string? s, params string[] matches) =>
        s.IsBlank() || matches.SkipBlanks().Any(m => s.Trim().Equals(m, StringComparison.OrdinalIgnoreCase));

    public static bool IsParenthesized(this string? s) {
        if (s.IsBlank())
            return false;
        s = s.Trim();
        int count = 0;
        if (!s.StartsWith("(", StringComparison.Ordinal)) return false;
        for (int i = 0; i < s.Length; i++) {
            char c = s[i];
            if (c == '(') {
                count++;
                continue;
            }

            if (c == ')') {
                count--;
                if (count == 0 && i + 1 < s.Length) return false;
            }
        }

        return count == 0;
    }

    public static string? NoUnderdashes(this string? value) =>
        value?.Replace("_", "", StringComparison.OrdinalIgnoreCase);

    public static string Parenthesize(this string s) =>
        s.ParenthesizeIf(!s.IsParenthesized());

    public static string ParenthesizeIf(this string s, bool condition = false) =>
        condition ? $"({s})" : s;

    public static string? PascalToCamelCase(this string? value) =>
        value.RegexReplace(@"(^\w)", ToLowerInvariant);

    public static string? PrefixedBy(this string? value, string prefix) =>
        value.IsBlank() ? null : prefix + value;

    public static string? RegexReplace(this string? value, string pattern, string replacement) =>
        value.SafeTransform(s => Regex.Replace(s, pattern, replacement));

    public static string? RegexReplace(this string? value, string pattern, MatchEvaluator me) =>
        value.SafeTransform(s => Regex.Replace(s, pattern, me));

    public static string Required([NotNull] this string? value, [CallerArgumentExpression(nameof(value))] string? name = null) =>
        value.IsBlank() ? throw ArgRequired(name) : value;

    public static string RequiredUsing([NotNull] this string? value, Func<string?, Exception> exceptor, [CallerArgumentExpression(nameof(value))] string? name = null) =>
        value.IsBlank() ? throw exceptor.Required()(name) : value;

    public static string Reversed(this string s) =>
        s.IsBlank() ? string.Empty : new string(s.ToCharArray().Reverse().ToArray());

    public static bool SafeEqualsTo(this string? s, string? other) =>
        s is null ? other is null : s.Equals(other, StringComparison.Ordinal);

    public static string? SafeTransform(this string? value, Func<string, string>? transform) =>
        transform is null ? value : value is null ? null : transform(value);

    public static T? SafeTransformTo<T>(this string? value, Func<string, T>? transform) =>
        transform is null || value is null ? default : transform(value);

    public static bool SafeTrimmedEqualsTo(this string s, string other) =>
        s.TrimToNull().SafeEqualsTo(other.TrimToNull());

    public static string SimplifyAsFileName([NotNull] this string? name) =>
        name.IsBlank()
            ? throw new ArgumentException($"Name is empty or null")
            : NameFilterRegex().Replace(name.Trim(), ".").ToLowerInvariant();

    public static string? SingleUnderdashes(this string? value) =>
        value.RegexReplace("__+", "_").RegexReplace("^_", "");

    public static IEnumerable<uint> SplitAsUints(this string? value, char splitChar) =>
        value.Safe().Trim().Split(splitChar).Select(s =>
        s.AsUint());

    public static TEnum ToEnum<TEnum>(this string? value, TEnum @default = default) where TEnum : struct =>
        value.IsBlank() || !Enum.TryParse<TEnum>(value, ignoreCase: true, out var c) ? @default : c;

    public static string ToKebabCase(this string value) {
        if (string.IsNullOrEmpty(value))
            return value;

        var sb = new StringBuilder();
        int i = 0;
        bool addDash = false;

        // handles beginning of string, breaks on first letter or digit. addDash might be better named "canAddDash"
        for (; i < value.Length; i++) {
            char ch = value[i];
            if (char.IsLetterOrDigit(ch)) {
                addDash = !char.IsUpper(ch);
                _ = sb.Append(char.ToLowerInvariant(ch));
                i++;
                break;
            }
        }

        // reusing i, start at the same place
        for (; i < value.Length; i++) {
            char ch = value[i];
            if (char.IsUpper(ch)) {
                if (addDash) {
                    addDash = false;
                    _ = sb.Append('-');
                }
                _ = sb.Append(char.ToLowerInvariant(ch));
            } else if (char.IsLetterOrDigit(ch)) {
                addDash = true;
                _ = sb.Append(ch);
            } else {
                //this converts all non letter/digits to dash - specifically periods and underscores. Is this needed?
                addDash = false;
                _ = sb.Append('-');
            }
        }

        return sb.ToString();
    }

    public static string TrimNumericSuffix(this string? value, char separator = '#') =>
        value.Safe().Trim().TrimEnd(separator, '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

    public static string? TrimToNull(this string? s) =>
        s.IsBlank() ? null : s.Trim();

    public static byte[] UTF8Bytes(this string? s) =>
        Encoding.UTF8.GetBytes(s.WithDefault(string.Empty));

    public static string WithDefault(this string? s, string @default) =>
        s.IsBlank() ? @default : s.Trim();

    public static string WithDefault(this string? s, Func<string>? resolver) =>
        s.IsBlank() ? resolver.Required()() : s.Trim();

    public static string? WithNumericSuffix(this string? value, uint suffix, char separator = '#') =>
        value.SafeTransformTo(s => $"{s.TrimNumericSuffix(separator)}{separator}{suffix:0}");

    public static string WithoutWhiteSpace(this string s) =>
        s.IsBlank() ? string.Empty : WhiteSpaceRegex().Replace(s, " ").Trim();

    private static string ToLowerInvariant(Match match) =>
        match.Value.ToLowerInvariant();

    private static string ToUpperInvariant(Match match) =>
        match.Value.ToUpperInvariant();

    [GeneratedRegex("""[\.\s\r\n<>\:"/\\|\?\*]+""")]
    private static partial Regex NameFilterRegex();

    [GeneratedRegex("""[\r\n\s]+""")]
    private static partial Regex WhiteSpaceRegex();

    public static T? FromJson<T>(this string json) =>
        string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json, DefaultJsonOptions);


}
