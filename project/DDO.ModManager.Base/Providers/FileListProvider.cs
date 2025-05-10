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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DDO.ModManager.Base.Models;
using MiniCommon.Extensions;
using MiniCommon.IO;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.ModManager.Base.NativePC.Providers;

public static class FileListProvider
{
    /// <summary>
    /// Gets the files within a directory and its subdirectories.
    /// </summary>
    public static List<string> GetFiles(Settings runtimeSettings)
    {
        List<string> files = [];
        foreach (string path in runtimeSettings!.SearchDirectories!)
            files = [.. files.Concat(VFS.GetFiles(path, runtimeSettings.SearchPattern!, SearchOption.AllDirectories))];
        return files;
    }

    /// <summary>
    /// Deletes the specified directory from FS current working directory.
    /// </summary>
    public static List<ModFile> FileList(Settings runtimeSettings, List<string> files, string filterRegex)
    {
        if (Validate.For.IsNull(runtimeSettings))
            return [];
        if (Validate.For.IsNull(runtimeSettings?.SearchDirectories))
            return [];
        if (Validate.For.IsNullOrWhiteSpace([runtimeSettings?.SearchPattern, runtimeSettings?.DisabledAffix]))
            return [];

        Regex? regex = null;
        try
        {
            regex = new Regex(filterRegex!, RegexOptions.IgnoreCase);
        }
        catch (Exception ex)
        {
            LogProvider.Error("log.unhandled.exception", ex.ToString());
        }

        if (Validate.For.IsNull(regex))
            return [];

        return
        [
            .. files
                .Where(file => regex!.IsMatch(file))
                .Select(file => new ModFile
                {
                    FileName = file.NormalizePath(),
                    IsEnabled = VFS.GetFileExtension(file) != runtimeSettings?.DisabledAffix,
                })
                .DistinctBy(f => f.FileName)
                .Take(runtimeSettings?.MaxFilesList ?? 0),
        ];
    }

    /// <summary>
    /// Toggle the files enabled state by appending or removing the disabled extension.
    /// </summary>
    public static string? ToggleFileState(
        Settings runtimeSettings,
        string? filepath,
        Action<string?, string?, bool?> action,
        bool enable
    )
    {
        if (Validate.For.IsNullOrWhiteSpace([filepath]))
            return filepath;

        bool isDisabled = VFS.GetFileExtension(filepath!) == runtimeSettings.DisabledAffix;

        if ((enable && isDisabled) || (!enable && !isDisabled))
        {
            string destination = enable
                ? filepath![..filepath!.LastIndexOf(runtimeSettings.DisabledAffix!)]
                : filepath + runtimeSettings.DisabledAffix;
            action(filepath, destination, enable);
            return destination;
        }

        return filepath;
    }
}
