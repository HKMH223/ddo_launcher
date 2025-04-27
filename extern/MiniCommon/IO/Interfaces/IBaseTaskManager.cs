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

namespace MiniCommon.IO.Interfaces;

public interface IBaseTaskManager
{
    /// <summary>
    /// Executes a function asynchronously with no return value.
    /// </summary>
    public abstract Task Run(Func<Task> function);

    /// <summary>
    /// Executes a function asynchronously with a return value and cancellation token.
    /// </summary>
    public abstract Task<TResult> Run<TResult>(
        Func<Task<TResult>> function,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Executes a function asynchronously with a return value.
    /// </summary>
    public abstract Task<TResult> Run<TResult>(Func<Task<TResult>> function);

    /// <summary>
    /// Executes a function synchronously with a return value and cancellation token.
    /// </summary>
    public abstract Task<TResult> Run<TResult>(
        Func<TResult> function,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Executes a function synchronously with a return value.
    /// </summary>
    public abstract Task<TResult> Run<TResult>(Func<TResult> function);

    /// <summary>
    /// Executes a function asynchronously with no return value and cancellation token.
    /// </summary>
    public abstract Task Run(Func<Task> function, CancellationToken cancellationToken);

    /// <summary>
    /// Executes an action asynchronously with a cancellation token.
    /// </summary>
    public abstract Task Run(Action action, CancellationToken cancellationToken);

    /// <summary>
    /// Executes an action asynchronously with no return value.
    /// </summary>
    public abstract Task Run(Action action);
}
