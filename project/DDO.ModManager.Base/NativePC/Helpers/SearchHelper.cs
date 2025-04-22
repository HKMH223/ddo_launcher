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
using DDO.ModManager.Base.NativePC.Models;
using MiniCommon.Extensions;
using MiniCommon.IO;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.ModManager.Base.NativePC.Helpers;

public static class SearchHelper
{
    /// <summary>
    /// Search for valid files based on a list of NtPcPath objects.
    /// </summary>
    public static (string, NtPcPath?) Search(string basePath, List<NtPcPath> paths)
    {
        if (Validate.For.IsNullOrWhiteSpace([basePath]))
            return (string.Empty, default);

        if (Validate.For.IsNullOrEmpty(paths))
            return (string.Empty, default);
#pragma warning disable S3267
        foreach (DirectoryInfo directory in VFS.GetDirectoryInfos(basePath, "*", SearchOption.AllDirectories))
#pragma warning restore S3267
        {
            string normalizedDir = directory.FullName.NormalizePath();
            string[] parts = normalizedDir.Split("/");
            bool? isDir = VFS.IsDirFile(normalizedDir);

            foreach (NtPcPath path in paths)
            {
                if (Validate.For.IsNull(path))
                    return (string.Empty, default);

                bool found = parts.Contains(path.Path);
                if (path.IgnoreCase)
                    found = parts.Contains(path.Path, StringComparer.CurrentCultureIgnoreCase);

                if (path.IsDir == true && found && isDir == true)
                    return (directory.FullName, path!);
            }
        }

        return SearchFilesOnly(basePath, paths, "*", SearchOption.AllDirectories);
    }

    /// <summary>
    /// Search for valid files based on a list of NtPcPath objects.
    /// </summary>
#pragma warning disable S3776
    public static (string, NtPcPath?) SearchFilesOnly(
        string basePath,
        List<NtPcPath> paths,
        string searchPattern,
        SearchOption searchOptions
    )
#pragma warning restore S3776
    {
        foreach (string file in VFS.GetFiles(basePath, searchPattern, searchOptions))
        {
            string fileName = VFS.Combine(basePath, VFS.GetFileName(file));
            string extension = VFS.GetFileExtension(fileName);

            if (VFS.IsDirFile(file) == true)
                continue;

            foreach (NtPcPath path in paths)
            {
                if (Validate.For.IsNull(path))
                    return (string.Empty, default);

                bool found = extension != path.Path;
                if (path.IgnoreCase)
                    found = !string.Equals(extension, path.Path, StringComparison.CurrentCultureIgnoreCase);
                if (path.IsDir == true || found)
                    continue;

                if (path.Unsupported == true)
                    LogProvider.Warn("ntpc.unsupported", extension, fileName);
                return (VFS.GetDirectoryName(fileName), path);
            }
        }

        return (string.Empty, default);
    }
}
