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

public static class IEnumerableOfClassExtensions
{
    public static IEnumerable<T> NonNulls<T>(this IEnumerable<T?>? values) where T : class
        => values.Safe().Skip(item => item is null)!;

    public static T[] NonEmpty<T>([NotNull] this T[] items, [CallerArgumentExpression(nameof(items))] string? parameterName = null) =>
        items is null || items.Length == 0 ? throw new ArgumentException("Should not be empty", parameterName) : items;
    public static bool AnyDefaults<T>(this IEnumerable<T?>? values) =>
     values.Safe().Required().Any(s => s.IsDefault());
    public static T[]? AllNonDefaults<T>(this T?[]? values, [CallerArgumentExpression(nameof(values))] string? name = null) =>
        AllNonDefaultsCore<T, T?[]?, T[]?>(values, name, list => list?.Select(s => s!).ToArray());
    public static IEnumerable<T>? AllNonDefaults<T>(this IEnumerable<T?>? values, [CallerArgumentExpression(nameof(values))] string? name = null) =>
        AllNonDefaultsCore<T, IEnumerable<T?>?, IEnumerable<T>?>(values, name, list => list?.Select(s => s!));
    public static IEnumerable<TResult> SelectByIndexSkippingNulls<TResult>(this IEnumerable<ulong> values, Func<ulong, TResult?> selector) where TResult : class =>
        values.Select(selector).SkipNulls();

    private static TTT AllNonDefaultsCore<T, TT, TTT>(TT values, string? name, Func<TT, TTT> assumer)
        where TT : IEnumerable<T?>?
        where TTT : IEnumerable<T>?
        =>
            AnyDefaults(values)
                ? throw new ArgumentException("Default value present", name)
                : assumer(values);



}
