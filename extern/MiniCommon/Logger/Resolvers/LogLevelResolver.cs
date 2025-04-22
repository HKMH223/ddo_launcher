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
using MiniCommon.Validation.Validators;

namespace MiniCommon.Logger.Resolvers;

public static class LogLevelResolver
{
    /// <summary>
    /// Convert NativeLogLevel to an name string.
    /// </summary>
    public static string ToString(NativeLogLevel? type) =>
        type switch
        {
            NativeLogLevel.Benchmark => "BENCHMARK",
            NativeLogLevel.Debug => "DEBUG",
            NativeLogLevel.Warn => "WARN",
            NativeLogLevel.Error => "ERROR",
            NativeLogLevel.Info => "INFO",
            NativeLogLevel.Native => "NATIVE",
            NativeLogLevel.Fatal => "FATAL",
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

    /// <summary>
    /// Convert a name string to NativeLogLevel.
    /// </summary>
    public static NativeLogLevel FromString(string? name)
    {
        if (Validate.For.IsNullOrWhiteSpace([name], NativeLogLevel.Fatal))
            return NativeLogLevel.Debug;

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
