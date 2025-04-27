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
using System.Threading;
using System.Threading.Tasks;
using MiniCommon.IO.Helpers;
using MiniCommon.IO.Interfaces;

namespace MiniCommon.IO;

public class TaskManager : ITaskManager
{
    public static ValidatedTaskManager BaseTaskManager { get; } = new();

    /// <inheritdoc />
    public static async Task Run(Func<Task> function)
    {
        await BaseTaskManager.Run(function);
    }

    /// <inheritdoc />
    public static async Task<TResult> Run<TResult>(
        Func<Task<TResult>> function,
        CancellationToken cancellationToken
    )
    {
        return await BaseTaskManager.Run(function, cancellationToken);
    }

    /// <inheritdoc />
    public static async Task<TResult> Run<TResult>(Func<Task<TResult>> function)
    {
        return await BaseTaskManager.Run(function);
    }

    /// <inheritdoc />
    public static async Task<TResult> Run<TResult>(
        Func<TResult> function,
        CancellationToken cancellationToken
    )
    {
        return await BaseTaskManager.Run(function, cancellationToken);
    }

    /// <inheritdoc />
    public static async Task<TResult> Run<TResult>(Func<TResult> function)
    {
        return await BaseTaskManager.Run(function);
    }

    /// <inheritdoc />
    public static async Task Run(Func<Task> function, CancellationToken cancellationToken)
    {
        await BaseTaskManager.Run(function, cancellationToken);
    }

    /// <inheritdoc />
    public static async Task Run(Action action, CancellationToken cancellationToken)
    {
        await BaseTaskManager.Run(action, cancellationToken);
    }

    /// <inheritdoc />
    public static async Task Run(Action action)
    {
        await BaseTaskManager.Run(action);
    }
}
