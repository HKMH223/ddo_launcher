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
using DDO.ModManager.Base.Enums;
using DDO.ModManager.Base.Models;
using DDO.ModManager.Base.NativePC.Providers;
using DDO.ModManager.Base.Resolvers;
using MiniCommon.CommandParser;
using MiniCommon.Extensions;
using MiniCommon.Interfaces;
using MiniCommon.IO;
using MiniCommon.IO.Enums;
using MiniCommon.IO.Helpers;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.ModManager.Base.Commands;

public class RunFileViewer(Settings _settings) : IBaseCommand
{
    private Settings RuntimeSettings { get; } = _settings;

    /// <summary>
    /// Run a file viewer data file.
    /// Additional parameters take the format of 'key=value' pairs.
    /// </summary>
    public Task Initialize(string[] args)
    {
        CommandLine.ProcessArgument(
            args,
            new()
            {
                Name = "run",
                Parameters = [new() { Name = "path", Optional = true }],
                Description = LocalizationProvider.Translate("command.run"),
            },
            options =>
            {
                if (Validate.For.IsNull(RuntimeSettings))
                    return;

                (string? fileViewerDataPath, string expectedFileViewerDataPath) =
                    JsonExtensionHelper.MaybeJsonWithComments(
                        VFS.GetRelativePath(VFS.FileSystem.Cwd, options.GetValueOrDefault("path", "data.json"))
                            .NormalizePath()
                    );

                if (fileViewerDataPath is null)
                {
                    LogProvider.Error("error.readfile", expectedFileViewerDataPath);
                    return;
                }
                FileViewerProvider.Process(RuntimeSettings, fileViewerDataPath!);
            }
        );

        return Task.CompletedTask;
    }
}
