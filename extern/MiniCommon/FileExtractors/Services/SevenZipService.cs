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
using MiniCommon.Extensions;
using MiniCommon.FileExtractors.Models;
using MiniCommon.FileExtractors.Resolvers;
using MiniCommon.IO.Helpers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace MiniCommon.FileExtractors.Services;

public class SevenZipService
{
    private SevenZipSettings? SevenZipSettings { get; }

    private SevenZipService() { }

    public SevenZipService(SevenZipSettings? sevenZipSettings)
    {
        SevenZipSettings = sevenZipSettings;
    }

    /// <summary>
    /// Extract an archive using SevenZip, using arguments specified by SevenZipSettings.
    /// </summary>
    public void Extract(string source, string destination)
    {
        string? args = SevenZipSettings
            .DefaultExtractArguments(source, destination)
            ?.Arguments()
            ?.Build();
        if (Validate.For.IsNullOrWhiteSpace([args]))
            return;

        ProcessHelper.RunProcess(
            SevenZipPathResolver.SevenZipPath(SevenZipSettings),
            string.Format(args!, source, destination),
            Environment.CurrentDirectory,
            SevenZipSettings!.UseShellExecute,
            SevenZipSettings!.Silent
        );
    }

    /// <summary>
    /// Compress data using SevenZip, using arguments specified by SevenZipSettings.
    /// </summary>
    public void Compress(string source, string destination)
    {
        string? args = SevenZipSettings
            .DefaultCompressionArguments(source, destination)
            ?.Arguments()
            ?.Build();
        if (Validate.For.IsNullOrWhiteSpace([args]))
            return;

        ProcessHelper.RunProcess(
            SevenZipPathResolver.SevenZipPath(SevenZipSettings),
            string.Format(args!, source, destination),
            Environment.CurrentDirectory,
            SevenZipSettings!.UseShellExecute,
            SevenZipSettings!.Silent
        );
    }
}
