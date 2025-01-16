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
using DDO.Launcher.Base.Helpers;
using DDO.Launcher.Base.Models;
using MiniCommon.CommandParser;
using MiniCommon.Interfaces;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.Commands;

public class Verifier : IBaseCommand<Settings>
{
    /// <summary>
    /// Verify file hashes.
    /// Additional parameters take the format of 'key=value' pairs.
    /// </summary>
    public Task Init(string[] args, Settings? settings)
    {
        CommandLine.ProcessArgument(
            args,
            new()
            {
                Name = "verify",
                Parameters = [new() { Name = "write", Optional = false }],
                Description = LocalizationProvider.Translate("command.verify"),
            },
            options =>
            {
                if (Validate.For.IsNull(settings))
                    return;

                if (!bool.TryParse(options.GetValueOrDefault("write", "true"), out bool write))
                    return;

                if (write)
                {
                    DDOVerifier.Write();
                    return;
                }

                DDOVerifier.Verify();
            }
        );

        return Task.CompletedTask;
    }
}
