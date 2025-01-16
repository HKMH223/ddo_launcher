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
using System.Threading.Tasks;
using DDO.Launcher.Base.Models;
using MiniCommon.CommandParser;
using MiniCommon.Interfaces;
using MiniCommon.IO;
using MiniCommon.Models;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.Commands;

public class Patcher : IBaseCommand<Settings>
{
    /// <summary>
    /// Patch a file from a list of Patch objects.
    /// Additional parameters take the format of 'key=value' pairs.
    /// </summary>
    public Task Init(string[] args, Settings? settings)
    {
        CommandLine.ProcessArgument(
            args,
            new()
            {
                Name = "patch",
                Parameters =
                [
                    new() { Name = "input", Optional = false },
                    new() { Name = "output", Optional = false },
                    new() { Name = "patch", Optional = false },
                ],
                Description = LocalizationProvider.Translate("command.patch"),
            },
            options =>
            {
                if (Validate.For.IsNull(settings))
                    return;

                string input = options.GetValueOrDefault("input", "");
                string output = options.GetValueOrDefault("output", "");
                string patch = options.GetValueOrDefault("patch", "");

                if (Validate.For.IsNullOrWhiteSpace([input, output, patch]))
                    return;

                if (!VFS.Exists(input))
                    return;

                if (!VFS.Exists(patch))
                    return;

                byte[] buffer = VFS.ReadFile(input);
                List<Patch> patches = Patch.ReadPatchList(patch);
                (int successful, byte[] modifiedBuffer) = Patch.Exec(buffer, patches);
                if (successful == 0)
                    return;
                VFS.WriteFile(output, modifiedBuffer);
            }
        );

        return Task.CompletedTask;
    }
}
