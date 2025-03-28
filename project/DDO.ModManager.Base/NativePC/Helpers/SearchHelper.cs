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
    public static (string, NtPcPath) Search(string basePath, List<NtPcPath> paths)
    {
        if (Validate.For.IsNullOrWhiteSpace([basePath]))
            return (string.Empty, new());

        DirectoryInfo[] directories = VFS.GetDirectoryInfos(basePath, "*", SearchOption.AllDirectories);

        foreach (DirectoryInfo directory in directories)
        {
            foreach (NtPcPath path in paths)
            {
                string fws = directory.FullName.NormalizePath();
                string[] parts = fws.Split("/");

                if (Validate.For.IsNull(path))
                    return (string.Empty, new());

                if (path.IsDir == true && parts.Contains(path.Path))
                    return (directory.FullName, path!);
            }
        }

        string[] files = VFS.GetFiles(basePath);

        foreach (string file in files)
        {
            foreach (NtPcPath path in paths)
            {
                if (Validate.For.IsNull(path))
                    return (string.Empty, new());

                string fileName = VFS.Combine(basePath, VFS.GetFileName(file));
                string extension = VFS.GetFileExtension(fileName);

                if (path.IsDir == true || VFS.IsDirFile(file) != false)
                    continue;
                if (extension != path.Path)
                    continue;

                if (path.Unsupported == true)
                    NotificationProvider.Warn("ntpc.unsupported", extension, fileName);
                return (VFS.GetDirectoryName(fileName), path);
            }
        }

        return (string.Empty, new());
    }
}
