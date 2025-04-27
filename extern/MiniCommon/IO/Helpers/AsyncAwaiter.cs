/*
 * DDO.Launcher
 * Copyright (C) 2024 DDO.Launcher Contributors
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MiniCommon.IO.Helpers;

public static class AsyncAwaiter
{
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

    /// <summary>
    /// Awaits the result of an asynchronous task.
    /// </summary>
    public static async Task<T> AwaitResultAsync<T>(
        string id,
        Func<Task<T>> task,
        int maxRequests = 1
    )
    {
        SemaphoreSlim semaphore = await GetOrCreateSemaphoreAsync(id, maxRequests);
        await semaphore.WaitAsync();

        try
        {
            T result = await task().ConfigureAwait(false);
            Cleanup(id);
            return result;
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Awaits the completion of an asynchronous task.
    /// </summary>
    public static async Task AwaitAsync(string id, Func<Task> task, int maxRequests = 1)
    {
        SemaphoreSlim semaphore = await GetOrCreateSemaphoreAsync(id, maxRequests);
        await semaphore.WaitAsync();

        try
        {
            await task().ConfigureAwait(false);
            Cleanup(id);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Retrieves or creates a semaphore for the specified identifier.
    /// </summary>
    private static async Task<SemaphoreSlim> GetOrCreateSemaphoreAsync(string id, int maxRequests)
    {
        await _semaphore.WaitAsync();
        try
        {
            return _semaphores.GetOrAdd(
                id,
                new SemaphoreSlim(Math.Max(maxRequests, 1), Math.Max(maxRequests, 1))
            );
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Cleans up the semaphore for a resource if the semaphore's current count is zero.
    /// </summary>
    private static void Cleanup(string id)
    {
        if (
            _semaphores.TryGetValue(id, out SemaphoreSlim? semaphore)
            && semaphore.CurrentCount <= 0
        )
        {
            _semaphores.TryRemove(id, out _);
        }
    }
}
