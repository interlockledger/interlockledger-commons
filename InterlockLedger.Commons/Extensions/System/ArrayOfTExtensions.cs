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

namespace System;
public static class ArrayOfTExtensions
{
    public static string JoinedBy<T>(this T[]? values, string joiner) =>
        string.Join(joiner, values.Safe().ToStrings());

    [return: NotNull]
    public static T[] OrEmpty<T>(this T[]? values) =>
        values ?? Array.Empty<T>();

    [return: NotNull]
    public static T[] MinLength<T>([NotNull] this T[] array, int length, [CallerArgumentExpression(nameof(array))] string? parameterName = null) =>
         array is not null && array.Length >= length
             ? array
             : throw new ArgumentException($"Array parameter {parameterName} must have length >= {length}");

    [return: NotNull]
    public static T[] MaxLength<T>([NotNull] this T[] array, int length, [CallerArgumentExpression(nameof(array))] string? parameterName = null) =>
         array is not null && array.Length <= length
             ? array
             : throw new ArgumentException($"Array parameter {parameterName} must have length <= {length}");

    [return: NotNull]
    public static T[] ExactLength<T>([NotNull] this T[] array, int length, [CallerArgumentExpression(nameof(array))] string? parameterName = null) =>
         array is not null && array.Length == length
             ? array
             : throw new ArgumentException($"Array parameter {parameterName} must have length == {length}");
}
