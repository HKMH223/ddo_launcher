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
using System.Text.Json.Serialization;
using MiniCommon.IO.Enums;

namespace MiniCommon.IO.Models;

public class PathCheck
{
    public string? Target { get; }
    public PathCheckType? Type { get; private set; }
    public PathCheckAction? Action { get; private set; }

    public PathCheck() { }

    public PathCheck(string target, PathCheckType checkType, PathCheckAction checkAction)
    {
        Target = target;
        Type = checkType;
        Action = checkAction;
    }

    public static List<PathCheck> DefaultProblemPaths()
    {
        return
        [
            // Problematic paths
            new("SteamApps", PathCheckType.EndsWith, PathCheckAction.Warn),
            new("Documents", PathCheckType.EndsWith, PathCheckAction.Warn),
            new("Desktop", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("Desktop", PathCheckType.Contains, PathCheckAction.Warn),
            new("scoped_dir", PathCheckType.Contains, PathCheckAction.Deny),
            new("Downloads", PathCheckType.Contains, PathCheckAction.Deny),
            new("OneDrive", PathCheckType.Contains, PathCheckAction.Deny),
            new("NextCloud", PathCheckType.Contains, PathCheckAction.Deny),
            new("DropBox", PathCheckType.Contains, PathCheckAction.Deny),
            new("Google", PathCheckType.Contains, PathCheckAction.Deny),
            new("Program Files", PathCheckType.Contains, PathCheckAction.Deny),
            new("Program Files (x86", PathCheckType.Contains, PathCheckAction.Deny),
            new("windows", PathCheckType.Contains, PathCheckAction.Deny),
            new("Drive Root", PathCheckType.DriveRoot, PathCheckAction.Deny),
            // Reserved words
            new("CON", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("PRN", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("AUX", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("CLOCK$", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("NUL", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("COM0", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("COM1", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("COM2", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("COM3", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("COM4", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("COM5", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("COM6", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("COM7", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("COM8", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("COM9", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("LPT0", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("LPT1", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("LPT2", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("LPT3", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("LPT4", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("LPT5", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("LPT6", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("LPT7", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("LPT8", PathCheckType.EndsWith, PathCheckAction.Deny),
            new("LPT9", PathCheckType.EndsWith, PathCheckAction.Deny),
        ];
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(PathCheck))]
internal partial class PathCheckContext : JsonSerializerContext;
