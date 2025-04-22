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

using System.Text.Json;
using System.Text.Json.Serialization;
using MiniCommon.Logger.Enums;
using MiniCommon.Validation;
using MiniCommon.Validation.Exceptions;
using MiniCommon.Validation.Validators;

namespace MiniCommon.IO.Abstractions;

public class ValidatedJson : BaseJson
{
    /// <inheritdoc />
    public override string Serialize<T>(T data, JsonSerializerOptions options)
    {
        if (Validate.For.IsNull(data, NativeLogLevel.Fatal))
            return string.Empty;
        return base.Serialize<T>(data, options);
    }

    /// <inheritdoc />
    public override string Serialize<T>(T data, JsonSerializerContext ctx)
    {
        if (Validate.For.IsNull(data, NativeLogLevel.Fatal))
            return string.Empty;
        return base.Serialize<T>(data, ctx);
    }

    /// <inheritdoc />
    public override T Deserialize<T>(string data, JsonSerializerOptions options)
    {
        if (Validate.For.IsNullOrWhiteSpace([data], NativeLogLevel.Fatal))
            throw new ObjectValidationException(nameof(data));
        return base.Deserialize<T>(data, options)!;
    }

    /// <inheritdoc />
    public override T Deserialize<T>(string data, JsonSerializerContext ctx)
    {
        if (Validate.For.IsNullOrWhiteSpace([data], NativeLogLevel.Fatal))
            throw new ObjectValidationException(nameof(data));
        return base.Deserialize<T>(data, ctx)!;
    }

    /// <inheritdoc />
    public override void Save<T>(string filepath, T data, JsonSerializerOptions options)
    {
        if (Validate.For.IsNullOrWhiteSpace([filepath], NativeLogLevel.Fatal))
            throw new ObjectValidationException(nameof(filepath));
        if (Validate.For.IsNull(data, NativeLogLevel.Fatal))
            throw new ObjectValidationException(nameof(data));
        base.Save(filepath, data, options);
    }

    /// <inheritdoc />
    public override void Save<T>(string filepath, T data, JsonSerializerContext ctx)
    {
        if (Validate.For.IsNullOrWhiteSpace([filepath], NativeLogLevel.Fatal))
            throw new ObjectValidationException(nameof(filepath));
        if (Validate.For.IsNull(data, NativeLogLevel.Fatal))
            throw new ObjectValidationException(nameof(data));
        base.Save(filepath, data, ctx);
    }

    /// <inheritdoc />
    public override T Load<T>(string filepath, JsonSerializerOptions options)
    {
        if (Validate.For.IsNullOrWhiteSpace([filepath], NativeLogLevel.Fatal))
            throw new ObjectValidationException(nameof(filepath));
        return base.Load<T>(filepath, options)!;
    }

    /// <inheritdoc />
    public override T Load<T>(string filepath, JsonSerializerContext ctx)
    {
        if (Validate.For.IsNullOrWhiteSpace([filepath], NativeLogLevel.Fatal))
            throw new ObjectValidationException(nameof(filepath));
        return base.Load<T>(filepath, ctx)!;
    }
}
