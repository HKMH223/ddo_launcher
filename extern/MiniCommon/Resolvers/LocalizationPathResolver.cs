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

using MiniCommon.IO;
using MiniCommon.IO.Enums;
using MiniCommon.IO.Helpers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace MiniCommon.Resolvers;

public static class LocalizationPathResolver
{
    /// <summary>
    /// The language file path specified by the language string.
    /// </summary>
    public static string? LanguageFilePath(string filepath, string language)
    {
        string? defaultPath =
            $"localization.{language}{JsonExtensionHelper.ToString(JsonExtensionType.Default)}";
        string? optionalPath = language + JsonExtensionHelper.ToString(JsonExtensionType.Default);
        if (Validate.For.IsNullOrWhiteSpace([defaultPath, optionalPath]))
            return null;
        return TryGetLanguageFilePath(filepath, defaultPath)
            ?? TryGetLanguageFilePath(filepath, optionalPath);
    }

    /// <summary>
    /// The default language file path specified by the language string.
    /// </summary>
    public static string DefaultLanguageFilePath(string filepath, string language) =>
        VFS.FromCwd(
            filepath,
            $"localization.{language}{JsonExtensionHelper.ToString(JsonExtensionType.Default)}"
        );

    /// <summary>
    /// Validate for a language file based on a base path and file name.
    /// </summary>
    public static string? TryGetLanguageFilePath(string basePath, string fileName)
    {
        string? path = JsonExtensionHelper
            .MaybeJsonWithComments(VFS.FromCwd(basePath, fileName))
            .Result;
        if (Validate.For.IsNullOrWhiteSpace([path]))
            return null;
        if (VFS.Exists(path!))
            return path;
        return null;
    }
}
