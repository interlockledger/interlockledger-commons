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

namespace System.Threading.Tasks;

public static class TaskExtensions
{
    public static async Task<Result> TryAsync(this Task task, Action<Exception>? errorHandler = null) {
        try {
            await task.ConfigureAwait(true);
            return Result.Ok;
        } catch (Exception ex) {
            errorHandler?.Invoke(ex);
            return ex;
        }
    }

    public static async Task<Result<T>> TryAsync<T>(this Task<T> task, Action<Exception>? errorHandler = null) where T : class {
        try {
            return await task.ConfigureAwait(true);
        } catch (Exception ex) {
            errorHandler?.Invoke(ex);
            return ex;
        }
    }

    public static Task WhenAllAsync(this IEnumerable<Task> tasks) =>
        tasks.None() ? Task.CompletedTask : Task.WhenAll(tasks);

    public static async Task<IEnumerable<T>> WhenAllAsync<T>(this IEnumerable<Task<T>> tasks) =>
        tasks.None() ? Enumerable.Empty<T>() : await Task.WhenAll(tasks).ConfigureAwait(false);

    public static async Task<IEnumerable<T>> WhenAllSequentialAsync<T>(this IEnumerable<Task<T>> tasks) {
        if (tasks.None())
            return Enumerable.Empty<T>();
        var results = new List<T>();
        foreach (var task in tasks)
            results.Add(await task.ConfigureAwait(false));
        return results;
    }

    public static async Task WhenAllSequentialAsync(this IEnumerable<Task> tasks) {
        if (tasks.SafeAny())
            foreach (var task in tasks)
                await task.ConfigureAwait(false);
    }

    public static async Task<IEnumerable<T>> WhenAllParallelAsync<T>(this IEnumerable<Task<T>> tasks, int degree) {
        if (tasks.None())
            return Enumerable.Empty<T>();
        var results = new List<T>();
        foreach (var chunk in tasks.Chunk(degree)) {
            var chunkResults = await Task.WhenAll(chunk).ConfigureAwait(false);
            results.AddRange(chunkResults);
        }
        return results;
    }

    public static async Task WhenAllParallelAsync(this IEnumerable<Task> tasks, int degree) {
        if (tasks.SafeAny())
            foreach (var chunk in tasks.Chunk(degree))
                await Task.WhenAll(chunk).ConfigureAwait(false);
    }

    public static async Task<TOut> MapAsync<TIn, TOut>(this Task<TIn> task, Func<TIn, Task<TOut>> mapAsync) =>
        await mapAsync.Required()(await task.Required().ConfigureAwait(false)).ConfigureAwait(false);

    public static async Task<TOut> MapAsync<TIn, TOut>(this Task<TIn> task, Func<TIn, TOut> map) =>
        map.Required()(await task.Required().ConfigureAwait(false));

    public static async Task<T> AlsoDoAsync<T>(this Task<T> task, Func<T, Task> extraAsync) {
        var res = await task.Required().ConfigureAwait(false);
        await extraAsync.Required()(res).ConfigureAwait(false);
        return res;
    }

    public static async Task<T> AlsoDoAsync<T>(this Task<T> task, Action<T> extra) {
        var res = await task.Required().ConfigureAwait(false);
        extra.Required()(res);
        return res;
    }
}
