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
using MiniCommon.IO.Enums;

namespace MiniCommon.IO.Helpers;

public static class JsonExtensionHelper
{
    private const string JsonExtension = ".json";

    /// <summary>
    /// Check if a path with the ".json" extension exists and return it, if not, check for ".jsonc" and return it.
    /// </summary>
    public static (string? Result, string Expected) MaybeJsonWithComments(string path)
    {
        if (
            !string.Equals(
                VFS.GetFileExtension(path),
                JsonExtension,
                StringComparison.CurrentCultureIgnoreCase
            )
        )
        {
            return (default, path);
        }

        string jsoncPath = path + "c";
        if (VFS.Exists(jsoncPath)) // ".jsonc"
            return (jsoncPath, path);

        if (VFS.Exists(path)) // ".json"
            return (path, path);

        return (default, path);
    }

    /// <summary>
    /// Convert JsonExtensionType to a file extension string.
    /// </summary>
    public static string ToString(JsonExtensionType type) =>
        type switch
        {
            JsonExtensionType.Default => ".json",
            JsonExtensionType.WithComments => ".jsonc",
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
}
