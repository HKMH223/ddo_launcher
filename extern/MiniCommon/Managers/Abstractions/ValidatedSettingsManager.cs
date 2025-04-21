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

using System.Text.Json.Serialization;
using MiniCommon.Logger.Enums;
using MiniCommon.Validation;
using MiniCommon.Validation.Exceptions;
using MiniCommon.Validation.Validators;

namespace MiniCommon.Managers.Abstractions;

public class ValidatedSettingsManager<T>(JsonSerializerContext _ctx) : BaseSettingsManager<T>(_ctx)
    where T : class
{
    /// <inheritdoc />
    public override void FirstRun(T data)
    {
        if (Validate.For.IsNull(data, NativeLogLevel.Fatal))
            throw new ObjectValidationException(nameof(data));
        base.FirstRun(data);
    }

    /// <inheritdoc />
    public override void Save(T data)
    {
        if (Validate.For.IsNull(data, NativeLogLevel.Fatal))
            throw new ObjectValidationException(nameof(data));
        base.Save(data);
    }

    /// <inheritdoc />
    public override T? Load()
    {
        return base.Load();
    }
}
