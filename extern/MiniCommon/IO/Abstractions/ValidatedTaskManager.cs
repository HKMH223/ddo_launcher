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
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Exceptions;
using MiniCommon.Validation.Validators;

namespace MiniCommon.IO.Helpers;

public class ValidatedTaskManager : BaseTaskManager
{
    /// <inheritdoc />
    public override async Task Run(Func<Task> function)
    {
        if (Validate.For.IsNull(function))
            throw new ObjectValidationException(nameof(function));
        try
        {
            await base.Run(function);
        }
        catch (Exception ex)
        {
            LogProvider.Error("log.unhandled.exception", ex.ToString());
            throw;
        }
    }

    /// <inheritdoc />
    public override async Task<TResult> Run<TResult>(
        Func<Task<TResult>> function,
        CancellationToken cancellationToken
    )
    {
        if (Validate.For.IsNull(function))
            throw new ObjectValidationException(nameof(function));
        try
        {
            return await base.Run(function, cancellationToken);
        }
        catch (Exception ex)
        {
            LogProvider.Error("log.unhandled.exception", ex.ToString());
            throw;
        }
    }

    /// <inheritdoc />
    public override async Task<TResult> Run<TResult>(Func<Task<TResult>> function)
    {
        if (Validate.For.IsNull(function))
            throw new ObjectValidationException(nameof(function));
        try
        {
            return await base.Run(function);
        }
        catch (Exception ex)
        {
            LogProvider.Error("log.unhandled.exception", ex.ToString());
            throw;
        }
    }

    /// <inheritdoc />
    public override async Task<TResult> Run<TResult>(
        Func<TResult> function,
        CancellationToken cancellationToken
    )
    {
        if (Validate.For.IsNull(function))
            throw new ObjectValidationException(nameof(function));
        try
        {
            return await base.Run(function, cancellationToken);
        }
        catch (Exception ex)
        {
            LogProvider.Error("log.unhandled.exception", ex.ToString());
            throw;
        }
    }

    /// <inheritdoc />
    public override async Task<TResult> Run<TResult>(Func<TResult> function)
    {
        if (Validate.For.IsNull(function))
            throw new ObjectValidationException(nameof(function));
        try
        {
            return await base.Run(function);
        }
        catch (Exception ex)
        {
            LogProvider.Error("log.unhandled.exception", ex.ToString());
            throw;
        }
    }

    /// <inheritdoc />
    public override async Task Run(Func<Task> function, CancellationToken cancellationToken)
    {
        if (Validate.For.IsNull(function))
            throw new ObjectValidationException(nameof(function));
        try
        {
            await base.Run(function, cancellationToken);
        }
        catch (Exception ex)
        {
            LogProvider.Error("log.unhandled.exception", ex.ToString());
            throw;
        }
    }

    /// <inheritdoc />
    public override async Task Run(Action action, CancellationToken cancellationToken)
    {
        if (Validate.For.IsNull(action))
            throw new ObjectValidationException(nameof(action));
        try
        {
            await base.Run(action, cancellationToken);
        }
        catch (Exception ex)
        {
            LogProvider.Error("log.unhandled.exception", ex.ToString());
            throw;
        }
    }

    /// <inheritdoc />
    public override async Task Run(Action action)
    {
        if (Validate.For.IsNull(action))
            throw new ObjectValidationException(nameof(action));
        try
        {
            await base.Run(action);
        }
        catch (Exception ex)
        {
            LogProvider.Error("log.unhandled.exception", ex.ToString());
            throw;
        }
    }
}
