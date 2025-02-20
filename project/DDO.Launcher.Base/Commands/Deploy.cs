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
using DDO.Launcher.Base.NativePC.Helpers;
using DDO.Launcher.Base.NativePC.Models;
using DDO.Launcher.Base.NativePC.Providers;
using MiniCommon.CommandParser;
using MiniCommon.Extensions;
using MiniCommon.Interfaces;
using MiniCommon.IO;
using MiniCommon.Logger.Enums;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.Commands;

public class Deploy : IBaseCommand<Settings>
{
    /// <summary>
    /// Deploy files to an output directory based on a user-specified game and rule JSON object.
    /// Additional parameters take the format of 'key=value' pairs.
    /// </summary>
    public Task Init(string[] args, Settings? settings)
    {
        CommandLine.ProcessArgument(
            args,
            new()
            {
                Name = "deploy",
                Parameters =
                [
                    new() { Name = "mods", Optional = true },
                    new() { Name = "temp", Optional = true },
                    new() { Name = "output", Optional = true },
                    new() { Name = "game", Optional = true },
                    new() { Name = "rules", Optional = true },
                ],
                Description = LocalizationProvider.Translate("command.deploy"),
            },
            options =>
            {
                string gamePath = VFS.GetRelativePath(
                        VFS.FileSystem.Cwd,
                        options.GetValueOrDefault("game", "game.json")
                    )
                    .NormalizePath();
                string rulePath = VFS.GetRelativePath(
                        VFS.FileSystem.Cwd,
                        options.GetValueOrDefault("rules", "rules.json")
                    )
                    .NormalizePath();

                if (!VFS.Exists(gamePath))
                {
                    NotificationProvider.Error("error.readfile", gamePath);
                    return;
                }

                if (!VFS.Exists(rulePath))
                {
                    NotificationProvider.Error("error.readfile", rulePath);
                    return;
                }

                NtPcGame game = NtPcGame.Read(gamePath);
                NtPcRules rules = NtPcRules.Read(rulePath);

                if (Validate.For.IsNull(game.Deploy, NativeLogLevel.Fatal))
                    return;

                if (
                    Validate.For.IsNullOrWhiteSpace(
                        [game.Deploy!.Mods, game.Deploy.Temp, game.Deploy.Output],
                        NativeLogLevel.Fatal
                    )
                )
                {
                    return;
                }

                string modPath = PathHelper
                    .MaybeCwd(options.GetValueOrDefault("mods", game.Deploy!.Mods!), VFS.FileSystem.Cwd)
                    .NormalizePath();
                string tempPath = PathHelper
                    .MaybeCwd(options.GetValueOrDefault("temp", game.Deploy!.Temp!), VFS.FileSystem.Cwd)
                    .NormalizePath();
                string outputPath = PathHelper
                    .MaybeCwd(options.GetValueOrDefault("output", game.Deploy!.Output!), VFS.FileSystem.Cwd)
                    .NormalizePath();

                if (!VFS.Exists(modPath))
                {
                    NotificationProvider.Error("error.readfile", modPath);
                    return;
                }

                ExtractHelper.Extract(modPath, tempPath, game);
                NtPcProvider.DeleteDirectory(outputPath);
                NtPcProvider.Deploy(tempPath, outputPath, game, rules);
                NtPcProvider.DeleteDirectory(tempPath);
            }
        );

        return Task.CompletedTask;
    }
}
