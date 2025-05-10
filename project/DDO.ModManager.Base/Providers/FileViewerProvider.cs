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
using System.Linq;
using DDO.ModManager.Base.Models;
using MiniCommon.IO;
using MiniCommon.Logger.Enums;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using MiniCommon.Validation.Validators;

namespace DDO.ModManager.Base.NativePC.Providers;

public static class FileViewerProvider
{
    /// <summary>
    /// Process a file viewer data file.
    /// </summary>
#pragma warning disable S3776
    public static void Process(Settings runtimeSettings, string filepath)
#pragma warning restore S3776
    {
        if (Validate.For.IsNullOrWhiteSpace([filepath]))
            return;
        if (!VFS.Exists(filepath))
            return;
        FileViewer? fileViewerData = FileViewer.Read(filepath);
        if (Validate.For.IsNull(fileViewerData))
            return;
        List<string> files = FileListProvider.GetFiles(runtimeSettings);

        foreach (FileViewerData data in fileViewerData?.Data ?? Validate.For.EmptyList<FileViewerData>())
        {
            if (Validate.For.IsNullOrWhiteSpace([data?.Regex]))
                return;
            List<ModFile> modFiles = FileListProvider.FileList(runtimeSettings, files, data!.Regex!);
            List<ModFile> filteredModFiles = [.. modFiles.Take(data.Take ?? modFiles.Count)];
            if (data.Index != null)
            {
                if (data.Index > filteredModFiles.Count)
                    return;
                filteredModFiles = [filteredModFiles[data.Index ?? 0]];
            }

            foreach (ModFile file in filteredModFiles)
            {
                FileListProvider.ToggleFileState(
                    runtimeSettings,
                    file.FileName,
                    (string? src, string? dst, bool? _) =>
                    {
                        if (Validate.For.IsNullOrWhiteSpace([src, dst], NativeLogLevel.Fatal))
                            return;
                        LogProvider.Info("ntpc.move", src!, dst!);
                        VFS.MoveFile(src!, dst!);
                    },
                    data.IsEnabled ?? true
                );
            }
        }
    }
}
