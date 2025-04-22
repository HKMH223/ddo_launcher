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
using System.Text.Json;
using System.Text.Json.Serialization;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;

namespace MiniCommon.Models;

public class MArguments
{
    public List<MOption> Arguments { get; set; } = [];
    private readonly List<string> parsedLaunchArgs = [];

    /// <summary>
    /// Add an MOption to the MArguments Arguments list.
    /// </summary>
    public void Add(string arg, string[]? argParams = null, int priority = 0) =>
        Arguments.Add(new(arg, argParams, priority));

    /// <summary>
    /// Build the MArguments Arguments list into a string of arguments.
    /// </summary>
    public string Build()
    {
        parsedLaunchArgs.Clear();
        foreach (MOption arg in (List<MOption>)([.. Arguments.OrderBy(a => a.Priority)]))
        {
            if (!arg.Ignore)
                parsedLaunchArgs.Add(arg.Parse() ?? Validate.For.EmptyString());
        }
        return string.Join(" ", parsedLaunchArgs);
    }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(MArguments))]
internal partial class MArgumentsContext : JsonSerializerContext;
