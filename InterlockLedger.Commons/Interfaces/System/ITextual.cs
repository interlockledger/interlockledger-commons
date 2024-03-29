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

public interface ITextual
{
    bool IsEmpty { get; }
    string TextualRepresentation { get; }
    string? InvalidityCause { get; }

    bool IsInvalid => !InvalidityCause.IsBlank();
    string FullRepresentation => IsInvalid ? TextualRepresentation + Environment.NewLine + InvalidityCause! : TextualRepresentation;
}

#pragma warning disable CA1000 // Do not declare static members on generic types
public interface ITextual<TSelf> : ITextual, IEquatable<TSelf> where TSelf : ITextual<TSelf>
{
    public ITextual<TSelf> Textual { get; }
    public static abstract TSelf Empty { get; }
    public static abstract Regex Mask { get; }
    public static abstract TSelf InvalidBy(string cause);
    public static abstract TSelf Build(string textualRepresentation);
    public static TSelf Parse(string? textualRepresentation) =>
        textualRepresentation.IsBlank() || textualRepresentation.SafeEqualsTo(TSelf.Empty.TextualRepresentation)
            ? TSelf.Empty
            : TSelf.Mask.IsMatch(textualRepresentation)
                ? TSelf.Build(textualRepresentation!)
                : TSelf.InvalidBy($"Input '{textualRepresentation}' does not match {TSelf.Mask}");

    public static string? Validate(string? textualRepresentation) {
        if (textualRepresentation.IsBlank())
            return "Missing value";
        var resolved = Parse(textualRepresentation);
        return resolved.IsInvalid ? resolved.InvalidityCause : null;
    }
}
