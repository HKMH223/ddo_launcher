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
using DDO.Launcher.Base.NativePC.Models;
using MiniCommon.Extensions;
using MiniCommon.IO;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.NativePC.Helpers;

public static class CopyHelper
{
    /// <summary>
    /// Returns true if the fileName is in the exclusion list.
    /// </summary>
    public static bool SkipCopyFiles(string fileName, List<string> exclusions)
    {
        if (exclusions.Count == 0)
            return false;

        return exclusions.Contains(fileName);
    }

    /// <summary>
    /// Returns true if the fileName is in the exclusion list.
    /// </summary>
    public static bool SkipCopyAddons(string fileName, List<string> exclusions)
    {
        if (exclusions.Count == 0)
            return false;

        foreach (string exclusion in exclusions)
        {
            if (exclusion.StartsWith("ext:"))
            {
                if (VFS.GetFileExtension(fileName) == exclusion.Replace("ext:", string.Empty))
                    return true;
            }
            else if (VFS.GetFileName(fileName) == exclusion)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Rename part of a file destination while copying.
    /// </summary>
    public static string RenameDestination(string source, string destination, NtPcRules rules)
    {
        foreach (NtPcRename rename in rules.Renames ?? Validate.For.EmptyList<NtPcRename>())
        {
            if (Validate.For.IsNull(rename))
                return string.Empty;

            if (Validate.For.IsNullOrWhiteSpace([rename.Name, rename.Old]))
                return string.Empty;

            if (rename.Name == source)
                return destination.Replace(rename.Old!, rename.New);
        }

        return destination;
    }

    /// <summary>
    /// Copy hooks according to specified rules.
    /// </summary>
    public static void CopyHooks(
        string source,
        string fileName,
        string destination,
        NtPcGame game,
        NtPcRules rules,
        List<string> exclusions,
        List<string> hookNames
    )
    {
        if (Validate.For.IsNull(game))
            return;

        if (Validate.For.IsNullOrWhiteSpace([game.Arch]))
            return;

        if (Validate.For.IsNull(game.Engine))
            return;

        if (Validate.For.IsNull(game.Engine!.Hooks))
            return;

        foreach (NtPcHook hook in game.Engine!.Hooks!)
        {
            string fullSource = VFS.Combine(source, VFS.GetFileName(fileName));

            if (
                VFS.GetFileName(fullSource) == hook.Name
                && (hook.Arch == game.Arch || game.Arch!.Equals("any", StringComparison.CurrentCultureIgnoreCase))
            )
            {
                string fullDestination = string.Empty;
                if (hook?.Requires?.Count != 0)
                {
                    fullDestination = VFS.Combine(
                        destination,
                        VFS.Combine([.. hook?.Requires ?? Validate.For.EmptyList<string>()]),
                        hook?.Dll ?? Validate.For.EmptyString()
                    );
                }
                else
                {
                    fullDestination = VFS.Combine(destination, hook?.Dll ?? Validate.For.EmptyString());
                }

                Copy(
                    VFS.GetFullPath(VFS.FromCwd(fileName)),
                    VFS.GetFullPath(VFS.FromCwd(fullDestination)),
                    VFS.GetFullPath(VFS.FromCwd(source)),
                    (src) => SkipCopyFiles(src, exclusions),
                    (dest) => RenameDestination(source, dest, rules),
                    [.. rules.IgnorePrefixes ?? Validate.For.EmptyList<string>()],
                    hookNames
                );
            }
        }
    }

    /// <summary>
    /// Copy files to the specified destination according to an NtPcPath.
    /// </summary>
    public static void CopyFiles(
        string source,
        string fileName,
        string destination,
        NtPcPath path,
        NtPcRules rules,
        List<string> exclusions,
        List<string> hookNames
    )
    {
        int indexOfSource = fileName.IndexOf(source);
        string searchDirectory = indexOfSource >= 0 ? fileName[..indexOfSource] + source : fileName;

        Copy(
            VFS.GetFullPath(VFS.FromCwd(fileName)),
            VFS.GetFullPath(VFS.FromCwd(FixDestination(destination, path))),
            VFS.GetFullPath(VFS.FromCwd(searchDirectory)),
            (src) => SkipCopyFiles(src, exclusions),
            (dest) => RenameDestination(source, dest, rules),
            [.. rules.IgnorePrefixes ?? Validate.For.EmptyList<string>()],
            hookNames
        );
    }

    /// <summary>
    /// Copy addons according to a list of NtPcAddon objects.
    /// </summary>
    public static void CopyAddons(
        string source,
        string fileName,
        string destination,
        NtPcRules rules,
        List<string> hookNames
    )
    {
        foreach (NtPcAddon addon in rules.Addons ?? Validate.For.EmptyList<NtPcAddon>())
        {
            if (Validate.For.IsNull(addon))
                return;

            if (addon.Name == fileName)
            {
                string absoluteSource = VFS.GetFullPath(
                    VFS.FromCwd(source, fileName, addon.Source ?? Validate.For.EmptyString())
                );
                string absoluteDestination = VFS.GetFullPath(
                    VFS.FromCwd(destination, addon.Destination ?? Validate.For.EmptyString())
                );

                Copy(
                    absoluteSource,
                    absoluteDestination,
                    absoluteSource,
                    (src) => SkipCopyAddons(src, addon.Skip ?? Validate.For.EmptyList<string>()),
                    (dest) => RenameDestination(absoluteSource, dest, rules),
                    [.. rules.IgnorePrefixes ?? Validate.For.EmptyList<string>()],
                    hookNames
                );
            }
        }
    }

    /// <summary>
    /// Copy addons after all other addons, according to a list of NtPcAddon objects.
    /// </summary>
    public static void CopyPostAddons(string source, string destination, NtPcRules rules, List<string> hookNames)
    {
        foreach (NtPcAddon addon in rules.Addons ?? Validate.For.EmptyList<NtPcAddon>())
        {
            if (Validate.For.IsNull(addon))
                return;

            if (addon.Name != "copy")
                continue;

            string absoluteSource = VFS.GetFullPath(VFS.FromCwd(source, addon.Source ?? Validate.For.EmptyString()));
            string absoluteDestination = VFS.GetFullPath(
                VFS.FromCwd(destination, addon.Destination ?? Validate.For.EmptyString())
            );

            Copy(
                absoluteSource,
                absoluteDestination,
                VFS.GetFullPath(VFS.FromCwd(source)),
                (src) => SkipCopyAddons(src, addon.Skip ?? Validate.For.EmptyList<string>()),
                (dest) => RenameDestination(source, dest, rules),
                [.. rules.IgnorePrefixes ?? Validate.For.EmptyList<string>()],
                hookNames
            );
        }
    }

    /// <summary>
    /// Copies a file or directory based on the specified functions.
    /// </summary>
    private static void Copy(
        string source,
        string destination,
        string searchDirectory,
        Func<string, bool> skip,
        Func<string, string> rename,
        string[] ignorePrefixes,
        List<string> hookNames
    )
    {
        bool? fileType = VFS.IsDirFile(source);

        switch (fileType)
        {
            case true: // Directory
                if (ignorePrefixes.Any(VFS.GetDirectoryName(source).StartsWith))
                    break;
                CopyDirectory(source, destination, searchDirectory, skip, rename, hookNames);
                break;
            case false:
                if (ignorePrefixes.Any(VFS.GetFileName(source).StartsWith))
                    break;
                CopyFile(source, destination, skip, rename, hookNames);
                break;
            default:
                NotificationProvider.Warn("ntpc.missing", source);
                break;
        }
    }

    /// <summary>
    /// Walk through a directory and copy all files that follow the specified functions.
    /// </summary>
    private static void CopyDirectory(
        string source,
        string destination,
        string searchDirectory,
        Func<string, bool> skip,
        Func<string, string> rename,
        List<string> hookNames
    )
    {
        FileInfo[] files = VFS.GetFileInfos(searchDirectory, "*", SearchOption.AllDirectories);

        foreach (FileInfo file in files)
        {
            string normalizedSource = source.NormalizePath();
            string normalizedDestination = destination.NormalizePath();
            string normalizedFilePath = file.FullName.NormalizePath();

            if (skip(normalizedFilePath))
                continue;

            string newDestination = rename(normalizedFilePath.Replace(normalizedSource, normalizedDestination));
            if (normalizedFilePath != newDestination)
            {
                if (hookNames.Contains(VFS.GetFileName(normalizedFilePath)))
                    continue;
                NotificationProvider.Info("ntpc.copy", normalizedFilePath, newDestination);
                VFS.CopyFile(normalizedFilePath, newDestination);
            }
        }
    }

    /// <summary>
    /// Copy a file based on the specified functions.
    /// </summary>
    private static void CopyFile(
        string source,
        string destination,
        Func<string, bool> skip,
        Func<string, string> rename,
        List<string> hookNames
    )
    {
        string normalizedSource = source.NormalizePath();
        string normalizedDestination = destination.NormalizePath();

        if (skip(normalizedSource))
            return;

        string newDestination = rename(normalizedDestination);
        if (normalizedSource != newDestination)
        {
            if (hookNames.Contains(VFS.GetFileName(normalizedSource)))
                return;
            NotificationProvider.Info("ntpc.copy", normalizedSource, newDestination);
            VFS.CopyFile(normalizedSource, newDestination);
        }
    }

    /// <summary>
    /// Fix the destination based on an output path and NtPcPath object.
    /// </summary>
    private static string FixDestination(string basePath, NtPcPath path)
    {
        if (Validate.For.IsNull(path))
            return string.Empty;

        if (path.Requires?.Count == 0)
            return VFS.Combine(basePath, path.Path!);
        return VFS.Combine(basePath, VFS.Combine([.. path?.Requires ?? Validate.For.EmptyList<string>()]));
    }
}
