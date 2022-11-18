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

namespace System;

public interface ITextual<T> : IEquatable<T> where T : notnull, ITextual<T>, new()
{
    #region Must Implement
    bool IsEmpty { get; }
    string TextualRepresentation { get; }
    string? InvalidityCause { get; init; }
    public static abstract T Empty { get; }

    protected static abstract Regex Mask { get; }
    protected static abstract T FromString(string textualRepresentation);
    protected bool EqualsForValidInstances(T other);

    #endregion

    #region Implemented
    public bool EqualForAnyInstances(T? other) =>
        other is not null
        && ((IsEmpty || other.IsEmpty)
            ? IsEmpty == other.IsEmpty
            : (IsInvalid || other.IsInvalid)
                ? IsInvalid == other.IsInvalid
                : EqualsForValidInstances(other));

    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out T result) {
        result = Parse(s);
        return !result.IsInvalid;
    }

    public static T Parse(string? textualRepresentation) =>
        textualRepresentation.IsBlank() || textualRepresentation.SafeEqualsTo(T.Empty.TextualRepresentation)
            ? T.Empty
            : T.Mask.IsMatch(textualRepresentation)
                ? T.FromString(textualRepresentation!)
                : InvalidBy(T.Mask.InvalidityByNotMatching(textualRepresentation));
    string FullRepresentation =>
        !IsInvalid
        ? TextualRepresentation
        : $"{TextualRepresentation}{Environment.NewLine}{InvalidityCause}";

    bool IsInvalid => InvalidityCause is not null;

    private static T InvalidBy(string cause) => new() { InvalidityCause = cause };

    #endregion
}
