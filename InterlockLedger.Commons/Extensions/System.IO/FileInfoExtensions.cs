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

namespace System.IO;

public static class FileInfoExtensions
{
    public static FileStream GetWritingStream(this FileInfo fileInfo, Action<FileInfo> onDispose) => new DisposableWritingStream(fileInfo, onDispose);

    public static async Task<string?> ReadToEndAsync(this FileInfo file) {
        if (!file.Required().Exists)
            return null;
        using var reader = file.OpenText();
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    private sealed class DisposableWritingStream(FileInfo fileInfo, Action<FileInfo> onDispose) : FileStream(fileInfo.Required().FullName, FileMode.CreateNew, FileAccess.Write)
    {
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing)
                _onDispose(_fileInfo);
        }

        private readonly FileInfo _fileInfo = fileInfo.Required();
        private readonly Action<FileInfo> _onDispose = onDispose.Required();
    }
}
