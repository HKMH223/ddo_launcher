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
using MiniCommon.IO;

namespace MiniCommon.BuildInfo;

public static class AssemblyConstants
{
    public static string AssemblyName { get; set; } = "AssemblyName";
    public static string DataDirectory { get; set; } = ".data";
    public static string LocalizationDirectory { get; set; } = "localization";
    public static string LogsDirectory { get; set; } = "logs";

    public static string LogFilePath() =>
        VFS.FromCwd(DataDirectory, LogsDirectory, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log");

    public static string LocalizationPath() => VFS.FromCwd(DataDirectory, LocalizationDirectory);

    public static List<string> WatermarkText() =>
        [AssemblyName, "This work is free of charge", "If you paid money, you were scammed"];
}
