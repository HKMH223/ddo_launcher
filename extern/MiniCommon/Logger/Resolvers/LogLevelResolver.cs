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
using MiniCommon.Logger.Enums;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.Resolvers;

public static class LogLevelResolver
{
    /// <summary>
    /// Convert NativeLogLevel to an name string.
    /// </summary>
    public static string ToString(NativeLogLevel? type) =>
        type switch
        {
            NativeLogLevel.Benchmark => "BENCHMARK" ?? Validate.For.EmptyString(),
            NativeLogLevel.Debug => "DEBUG" ?? Validate.For.EmptyString(),
            NativeLogLevel.Warn => "WARN" ?? Validate.For.EmptyString(),
            NativeLogLevel.Error => "ERROR" ?? Validate.For.EmptyString(),
            NativeLogLevel.Info => "INFO" ?? Validate.For.EmptyString(),
            NativeLogLevel.Native => "NATIVE" ?? Validate.For.EmptyString(),
            NativeLogLevel.Fatal => "FATAL" ?? Validate.For.EmptyString(),
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

    /// <summary>
    /// Convert a name string to NativeLogLevel.
    /// </summary>
    public static NativeLogLevel FromString(string? name)
    {
        if (Validate.For.IsNullOrWhiteSpace([name], NativeLogLevel.Fatal))
            return default;

        return name!.ToUpperInvariant() switch
        {
            "BENCHMARK" => NativeLogLevel.Benchmark,
            "DEBUG" => NativeLogLevel.Debug,
            "WARN" => NativeLogLevel.Warn,
            "ERROR" => NativeLogLevel.Error,
            "INFO" => NativeLogLevel.Info,
            "NATIVE" => NativeLogLevel.Native,
            "FATAL" => NativeLogLevel.Fatal,
            _ => throw new ArgumentOutOfRangeException(nameof(name)),
        };
    }
}
