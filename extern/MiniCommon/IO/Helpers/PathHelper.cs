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

namespace MiniCommon.IO.Helpers;

public static class PathHelper
{
    /// <summary>
    /// Parse the path based on whether the "cwd:" prefix is used.
    /// </summary>
    public static string MaybeCwd(string path, string workingDirectory)
    {
        if (path.StartsWith("cwd:"))
            return VFS.Combine(workingDirectory, path.Replace("cwd:", string.Empty));
        return path;
    }
}
