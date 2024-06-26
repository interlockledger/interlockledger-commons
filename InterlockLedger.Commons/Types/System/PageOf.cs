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

#pragma warning disable CA1000 // Do not declare static members on generic types
public class PageOf<T>(IEnumerable<T> items, ushort page, byte pageSize, ushort totalNumberOfPages, bool lastToFirst) : IEquatable<PageOf<T>>
{
    public IEnumerable<T> Items { get; set; } = items;
    public ushort Page { get; set; } = page;
    public byte PageSize { get; set; } = pageSize;
    public ushort TotalNumberOfPages { get; set; } = totalNumberOfPages;
    public bool LastToFirst { get; set; } = lastToFirst;

    public PageOf() : this([], 0, 0, 0, false) { }
    public PageOf(IEnumerable<T> items, bool lastToFirst) : this(items.Required(), 0, 0, (ushort)(items.SafeAny() ? 1 : 0), lastToFirst) {
    }

    public static PageOf<T> Empty { get; } = new PageOf<T>();

    public override bool Equals(object? obj) => Equals(obj as PageOf<T>);
    public bool Equals(PageOf<T>? other) =>
        other is not null &&
        other.Page == Page &&
        other.PageSize == PageSize &&
        other.LastToFirst == LastToFirst &&
        other.TotalNumberOfPages == TotalNumberOfPages &&
        other.Items.SequenceEqual(Items);
    public override int GetHashCode() => HashCode.Combine(Items, Page, PageSize, TotalNumberOfPages, LastToFirst);

    public static bool operator ==(PageOf<T>? left, PageOf<T>? right) => EqualityComparer<PageOf<T>>.Default.Equals(left, right);
    public static bool operator !=(PageOf<T>? left, PageOf<T>? right) => !(left == right);
}
