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
public interface ITextual
{
    #region Must Implement
    bool IsEmpty { get; }
    string TextualRepresentation { get; }
    string? InvalidityCause { get; }
    public static abstract string InvalidTextualRepresentation { get; }
    public static abstract Regex Mask { get; }

    #endregion

    #region Implemented
    string FullRepresentation => !IsInvalid
         ? TextualRepresentation
         : $"{TextualRepresentation}{Environment.NewLine}{InvalidityCause}";

    bool IsInvalid => InvalidityCause is not null;

    #endregion
}

public interface ITextual<TSelf> : ITextual, IEquatable<TSelf> where TSelf : ITextual<TSelf>
{
    #region Must Implement
    bool EqualsForValidInstances(TSelf other);
    protected static abstract TSelf Empty { get; }
    protected static abstract TSelf FromString(string textualRepresentation);
    protected static abstract TSelf New(string? invalidityCause, string textualRepresentation);
    #endregion

    #region Implemented
    bool IEquatable<TSelf>.Equals(TSelf? other) =>
        other is not null
        && ((IsEmpty || other.IsEmpty)
            ? IsEmpty == other.IsEmpty
            : (IsInvalid || other.IsInvalid)
                ? IsInvalid == other.IsInvalid
                : EqualsForValidInstances(other));

    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out TSelf result) {
        result = Parse(s);
        return !result.IsInvalid;
    }

    public static TSelf Parse(string? textualRepresentation) =>
        MatchesEmptyRepresentation(textualRepresentation)
            ? TSelf.Empty
            : textualRepresentation is null
                ? ITextual<TSelf>.InvalidBy("Null can't be accepted")
                : !TSelf.Mask.IsMatch(textualRepresentation)
                    ? ITextual<TSelf>.InvalidBy(TSelf.Mask.InvalidityByNotMatching(textualRepresentation))
                    : TSelf.FromString(textualRepresentation!);

    public static TSelf InvalidBy(string cause) =>
        TSelf.New(cause, TSelf.InvalidTextualRepresentation);

    private static bool MatchesEmptyRepresentation(string? textualRepresentation) =>
        textualRepresentation.Safe().SafeTrimmedEqualsTo(TSelf.Empty.TextualRepresentation.Safe());

    #endregion
}
