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
using System.IO;
using System.Threading.Tasks;
using CodeAnalyzers.Analyzers;
using MiniCommon.CommandParser;
using MiniCommon.Interfaces;
using MiniCommon.IO;
using MiniCommon.Models;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;

namespace CodeAnalyzers.Commands;

public class AnalyzeCode : IBaseCommand
{
    public Task Initialize(string[] args)
    {
        CommandLine.ProcessArgument(
            args,
            new()
            {
                Name = "analyze",
                Parameters =
                [
                    new() { Name = "path", Optional = false },
                    new() { Name = "namespace", Optional = true },
                    new() { Name = "license", Optional = true },
                ],
            },
            options =>
            {
                string filepath = options.GetValueOrDefault("path", "./");
                if (!filepath.EndsWith(".csproj") || !VFS.Exists(filepath))
                    return;
                string[] files = VFS.GetFiles(
                    VFS.GetDirectoryName(filepath),
                    "*.cs",
                    SearchOption.AllDirectories
                );
                LicenseAnalyzer.Analyze(
                    files,
                    options.GetValueOrDefault("license", "LICENSE-NOTICE.txt")
                );
                NamespaceAnalyzer.Analyze(
                    files,
                    options.GetValueOrDefault("namespace", string.Empty)
                );
                LocalizationKeyAnalyzer.Analyze(
                    files,
                    LocalizationProvider.Localization ?? Validate.For.EmptyClass<Localization>()
                );
            }
        );

        return Task.CompletedTask;
    }
}
