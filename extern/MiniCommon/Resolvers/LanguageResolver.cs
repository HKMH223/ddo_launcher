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
using MiniCommon.Enums;
using MiniCommon.Logger.Enums;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace MiniCommon.Resolvers;

public static class LanguageResolver
{
    /// <summary>
    /// Convert Language to a two character language code.
    /// </summary>
    public static string ToString(Language type) =>
        type switch
        {
            Language.ENGLISH => "en",
            Language.CHINESE => "cn",
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

    /// <summary>
    /// Convert a two character language code to Language.
    /// </summary>
    public static Language FromString(string? code)
    {
        if (Validate.For.IsNullOrWhiteSpace([code], NativeLogLevel.Fatal))
            return default;

        return code!.ToLowerInvariant() switch
        {
            "en" => Language.ENGLISH,
            "cn" => Language.CHINESE,
            _ => throw new ArgumentOutOfRangeException(nameof(code)),
        };
    }
}
