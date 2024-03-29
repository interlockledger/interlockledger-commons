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

namespace System.Collections.Generic;
public static class IEnumerableOfStringExtensions
{
    public static string AsLines(this IEnumerable<string>? source) =>
        source.JoinedBy(Environment.NewLine);
    public static bool AnyBlanks(this IEnumerable<string?>? values) =>
    values.Safe().Any(s => s.IsBlank());
    public static string[]? AllNonBlanks(this string?[]? values, [CallerArgumentExpression(nameof(values))] string? name = null) =>
        AllNonBlanksCore(values, name, list => list?.Select(s => s!).ToArray());
    public static IEnumerable<string>? AllNonBlanks<T>(this IEnumerable<string?>? values, [CallerArgumentExpression(nameof(values))] string? name = null) =>
        AllNonBlanksCore(values, name, list => list?.Select(s => s!));
    private static TTT AllNonBlanksCore<TT, TTT>(TT values, string? name, Func<TT, TTT> assumer)
        where TT : IEnumerable<string?>?
        where TTT : IEnumerable<string>?
        =>
            AnyBlanks(values)
                ? throw new ArgumentException("Blank value present", name)
                : assumer(values);

}
