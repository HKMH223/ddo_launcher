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
using System.Text.Json;
using System.Text.Json.Serialization;
using MiniCommon.IO;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.ModManager.Base.Models;

public class FileViewer
{
    public List<FileViewerData>? Data { get; set; }

    /// <summary>
    /// Read and deserialize a file as a FileViewer object.
    /// </summary>
    public static FileViewer? Read(string path)
    {
        if (Validate.For.IsNullOrWhiteSpace([path]))
            return null;
        FileViewer? fileViewerData = Json.Deserialize<FileViewer>(VFS.ReadAllText(path), FileViewerContext.Default);
        if (Validate.For.IsNull(fileViewerData))
            return null;
        return fileViewerData!;
    }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(FileViewer))]
internal partial class FileViewerContext : JsonSerializerContext;
