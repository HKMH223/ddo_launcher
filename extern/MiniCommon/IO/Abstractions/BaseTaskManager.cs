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
using MiniCommon.IO.Interfaces;

namespace MiniCommon.IO.Helpers;

public class BaseTaskManager : IBaseTaskManager
{
    /// <inheritdoc />
    public virtual async Task Run(Func<Task> function) => await Task.Run(function);

    /// <inheritdoc />
    public virtual async Task<TResult> Run<TResult>(
        Func<Task<TResult>> function,
        CancellationToken cancellationToken
    ) => await Task.Run(function, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<TResult> Run<TResult>(Func<Task<TResult>> function) =>
        await Task.Run(function);

    /// <inheritdoc />
    public virtual async Task<TResult> Run<TResult>(
        Func<TResult> function,
        CancellationToken cancellationToken
    ) => await Task.Run(function, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<TResult> Run<TResult>(Func<TResult> function) =>
        await Task.Run(function);

    /// <inheritdoc />
    public virtual async Task Run(Func<Task> function, CancellationToken cancellationToken) =>
        await Task.Run(function, cancellationToken);

    /// <inheritdoc />
    public virtual async Task Run(Action action, CancellationToken cancellationToken) =>
        await Task.Run(action, cancellationToken);

    /// <inheritdoc />
    public virtual async Task Run(Action action) => await Task.Run(action);
}
