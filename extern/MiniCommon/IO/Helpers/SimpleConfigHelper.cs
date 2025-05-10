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

using System.Collections.Generic;
using System.Linq;
using MiniCommon.BuildInfo;
using MiniCommon.Extensions;
using MiniCommon.IO.Models;
using MiniCommon.Validation.Exceptions;

namespace MiniCommon.IO.Helpers;

public static class SimpleConfigHelper
{
    /// <summary>
    /// Read a text file configuration from {AssemblyConstants.AssemblyName}.txt.
    /// </summary>
    public static SimpleConfig Read(Dictionary<string, string> fallback)
    {
        string filename = $"{AssemblyConstants.AssemblyName}.txt";
        if (!VFS.Exists(filename))
            return new() { Dict = fallback };
        return new()
        {
            Dict = VFS.ReadAllLines(filename)
                .SelectMany(line => line.ParseKeyValuePairs())
                .ToDictionary(pair => pair.Key, pair => pair.Value),
        };
    }

    /// <summary>
    /// Try to get a value from the SimpleConfig dictionary.
    /// </summary>
    public static string GetValue(this SimpleConfig config, string key)
    {
        if (config.Dict.TryGetValue(key, out string? value))
            return value;
        throw new ObjectValidationException(nameof(value));
    }
}
