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

using System.IO;
using DDO.Launcher.Base.NativePC.Models;
using MiniCommon.FileExtractors.Services;
using MiniCommon.IO;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.NativePC.Helpers;

public static class ExtractHelper
{
    /// <summary>
    /// Extract files to a directory and copy non-archive format files to the directory.
    /// </summary>
    public static void Extract(string basePath, string outputPath, NtPcGame game)
    {
        if (Validate.For.IsNull(game))
            return;

        if (Validate.For.IsNull(game.Formats))
            return;

        FileInfo[] files = VFS.GetFileInfos(basePath, "*", SearchOption.AllDirectories);
        SevenZipService sevenZip = new(new());

        foreach (FileInfo file in files)
        {
            if (game.Formats!.Contains(file.Extension))
            {
                sevenZip.Extract(file.FullName, outputPath);
                continue;
            }

            VFS.CopyFile(file.FullName, VFS.GetFullPath(VFS.FromCwd(outputPath, file.Name)), true);
        }
    }
}
