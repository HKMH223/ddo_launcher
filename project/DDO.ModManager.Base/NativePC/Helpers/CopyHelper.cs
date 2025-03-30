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
using DDO.ModManager.Base.NativePC.Helpers.Models;
using DDO.ModManager.Base.NativePC.Models;
using MiniCommon.Extensions;
using MiniCommon.Extensions.Cryptography.Helpers;
using MiniCommon.IO;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using MiniCommon.Validation.Validators;

namespace DDO.ModManager.Base.NativePC.Helpers;

public static class CopyHelper
{
    /// <summary>
    /// Create CRC32 based on the "maybe" bool".
    /// </summary>
    public static string MaybeCreateCRC32(string fileName, bool formatting, bool maybe)
    {
        if (maybe)
            return CryptographyHelperExt.CreateCRC32(fileName, formatting);
        return string.Empty;
    }

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
    public static NtPcFileMap? CopyHooks(CopyHelperOptions options)
    {
        if (Validate.For.IsNull(options.NtPcGame))
            return default;

        if (Validate.For.IsNullOrWhiteSpace([options.NtPcGame!.Arch]))
            return default;

        if (Validate.For.IsNull(options.NtPcGame.Engine))
            return default;

        if (Validate.For.IsNull(options.NtPcGame.Engine!.Hooks))
            return default;

        List<NtPcFile> ntPcFiles = [];
        foreach (NtPcHook hook in options.NtPcGame.Engine!.Hooks!)
        {
            string fullSource = VFS.Combine(options.Source!, VFS.GetFileName(options.FileName!));

            if (
                VFS.GetFileName(fullSource) == hook.Name
                && (
                    hook.Arch == options.NtPcGame.Arch
                    || options.NtPcGame.Arch!.Equals("any", StringComparison.CurrentCultureIgnoreCase)
                )
            )
            {
                string fullDestination = string.Empty;
                if (hook?.Requires?.Count != 0)
                {
                    fullDestination = VFS.Combine(
                        options.Destination!,
                        VFS.Combine([.. hook?.Requires ?? Validate.For.EmptyList<string>()]),
                        hook?.Dll ?? Validate.For.EmptyString()
                    );
                }
                else
                {
                    fullDestination = VFS.Combine(options.Destination!, hook?.Dll ?? Validate.For.EmptyString());
                }

                ntPcFiles =
                [
                    .. ntPcFiles,
                    .. Copy(
                        new()
                        {
                            Source = VFS.GetFullPath(VFS.FromCwd(options.FileName!)),
                            Destination = VFS.GetFullPath(VFS.FromCwd(fullDestination)),
                            SearchDirectory = VFS.GetFullPath(VFS.FromCwd(options.Source!)),
                            Skip = (src) => SkipCopyFiles(src, options.Exclusions!),
                            Rename = (dest) => RenameDestination(options.Source!, dest, options.NtPcRules!),
                            IgnorePrefixes = [.. options.NtPcRules!.IgnorePrefixes ?? Validate.For.EmptyList<string>()],
                            HookNames = options.HookNames!,
                            CreateCRC32s = options.CreateCRC32s,
                        }
                    ),
                ];
            }
        }

        if (ntPcFiles.Count <= 0)
            return default;
        return new() { Source = options.Source, Files = ntPcFiles };
    }

    /// <summary>
    /// Copy files to the specified destination according to an NtPcPath.
    /// </summary>
    public static NtPcFileMap? CopyFiles(CopyHelperOptions options)
    {
        int indexOfSource = options.FileName!.IndexOf(options.Source!);
        string searchDirectory =
            indexOfSource >= 0 ? options.FileName![..indexOfSource] + options.Source : options.FileName;

        List<NtPcFile> ntPcFiles = Copy(
            new()
            {
                Source = VFS.GetFullPath(VFS.FromCwd(options.FileName!)),
                Destination = VFS.GetFullPath(VFS.FromCwd(FixDestination(options.Destination!, options.NtPcPath!))),
                SearchDirectory = VFS.GetFullPath(VFS.FromCwd(searchDirectory)),
                Skip = (src) => SkipCopyFiles(src, options.Exclusions!),
                Rename = (dest) => RenameDestination(options.Source!, dest, options.NtPcRules!),
                IgnorePrefixes = [.. options.NtPcRules!.IgnorePrefixes ?? Validate.For.EmptyList<string>()],
                HookNames = options.HookNames!,
                CreateCRC32s = options.CreateCRC32s,
            }
        );

        if (ntPcFiles.Count <= 0)
            return default;
        return new() { Source = options.Source, Files = ntPcFiles };
    }

    /// <summary>
    /// Copy addons according to a list of NtPcAddon objects.
    /// </summary>
    public static NtPcFileMap? CopyAddons(CopyHelperOptions options)
    {
        List<NtPcFile> ntPcFiles = [];
        foreach (NtPcAddon addon in options.NtPcRules!.Addons ?? Validate.For.EmptyList<NtPcAddon>())
        {
            if (Validate.For.IsNull(addon))
                return default;

            if (addon.Name == options.FileName!)
            {
                string absoluteSource = VFS.GetFullPath(
                    VFS.FromCwd(options.Source!, options.FileName!, addon.Source ?? Validate.For.EmptyString())
                );
                string absoluteDestination = VFS.GetFullPath(
                    VFS.FromCwd(options.Destination!, addon.Destination ?? Validate.For.EmptyString())
                );

                ntPcFiles =
                [
                    .. ntPcFiles,
                    .. Copy(
                        new()
                        {
                            Source = absoluteSource,
                            Destination = absoluteDestination,
                            SearchDirectory = absoluteSource,
                            Skip = (src) => SkipCopyAddons(src, addon.Skip ?? Validate.For.EmptyList<string>()),
                            Rename = (dest) => RenameDestination(absoluteSource, dest, options.NtPcRules!),
                            IgnorePrefixes = [.. options.NtPcRules!.IgnorePrefixes ?? Validate.For.EmptyList<string>()],
                            HookNames = options.HookNames!,
                            CreateCRC32s = options.CreateCRC32s,
                        }
                    ),
                ];
            }
        }

        if (ntPcFiles.Count <= 0)
            return default;
        return new() { Source = options.Source, Files = ntPcFiles };
    }

    /// <summary>
    /// Copy addons after all other addons, according to a list of NtPcAddon objects.
    /// </summary>
    public static NtPcFileMap? CopyPostAddons(CopyHelperOptions options)
    {
        List<NtPcFile> ntPcFiles = [];
        foreach (NtPcAddon addon in options.NtPcRules!.Addons ?? Validate.For.EmptyList<NtPcAddon>())
        {
            if (Validate.For.IsNull(addon))
                return default;

            if (addon.Name != "copy")
                continue;

            string absoluteSource = VFS.GetFullPath(
                VFS.FromCwd(options.Source!, addon.Source ?? Validate.For.EmptyString())
            );
            string absoluteDestination = VFS.GetFullPath(
                VFS.FromCwd(options.Destination!, addon.Destination ?? Validate.For.EmptyString())
            );

            Copy(
                new()
                {
                    Source = absoluteSource,
                    Destination = absoluteDestination,
                    SearchDirectory = VFS.GetFullPath(VFS.FromCwd(options.Source!)),
                    Skip = (src) => SkipCopyAddons(src, addon.Skip ?? Validate.For.EmptyList<string>()),
                    Rename = (dest) => RenameDestination(options.Source!, dest, options.NtPcRules!),
                    IgnorePrefixes = [.. options.NtPcRules!.IgnorePrefixes ?? Validate.For.EmptyList<string>()],
                    HookNames = options.HookNames!,
                    CreateCRC32s = options.CreateCRC32s,
                }
            );
        }

        if (ntPcFiles.Count <= 0)
            return default;
        return new() { Source = options.Source, Files = ntPcFiles };
    }

    /// <summary>
    /// Copies a file or directory based on the specified functions.
    /// </summary>
    private static List<NtPcFile> Copy(CopyOptions options)
    {
        bool? fileType = VFS.IsDirFile(options.Source!);

        switch (fileType)
        {
            case true: // Directory
                if (options.IgnorePrefixes!.Any(VFS.GetDirectoryName(options.Source!).StartsWith))
                    break;
                return CopyDirectory(options);
            case false:
                if (options.IgnorePrefixes!.Any(VFS.GetFileName(options.Source!).StartsWith))
                    break;
                return CopyFile(options);
            default:
                NotificationProvider.Warn("ntpc.missing", options.Source!);
                return [];
        }

        return [];
    }

    /// <summary>
    /// Walk through a directory and copy all files that follow the specified functions.
    /// </summary>
    private static List<NtPcFile> CopyDirectory(CopyOptions options)
    {
        List<NtPcFile> ntPcFiles = [];
        FileInfo[] files = VFS.GetFileInfos(options.SearchDirectory!, "*", SearchOption.AllDirectories);

        foreach (FileInfo file in files)
        {
            string normalizedSource = options.Source!.NormalizePath();
            string normalizedDestination = options.Destination!.NormalizePath();
            string normalizedFilePath = file.FullName.NormalizePath();

            if (options.Skip!(normalizedFilePath))
                continue;

            string newDestination = options.Rename!(
                normalizedFilePath.Replace(normalizedSource, normalizedDestination)
            );
            if (normalizedFilePath != newDestination)
            {
                if (options.HookNames!.Contains(VFS.GetFileName(normalizedFilePath)))
                    continue;
                NotificationProvider.Info("ntpc.copy", normalizedFilePath, newDestination);
                VFS.CopyFile(normalizedFilePath, newDestination);
                ntPcFiles.Add(
                    new()
                    {
                        Source = normalizedFilePath,
                        Destination = newDestination,
                        Crc = MaybeCreateCRC32(normalizedFilePath, true, options.CreateCRC32s),
                    }
                );
            }
        }

        return ntPcFiles;
    }

    /// <summary>
    /// Copy a file based on the specified functions.
    /// </summary>
    private static List<NtPcFile> CopyFile(CopyOptions options)
    {
        List<NtPcFile> ntPcFiles = [];
        string normalizedSource = options.Source!.NormalizePath();
        string normalizedDestination = options.Destination!.NormalizePath();

        if (options.Skip!(normalizedSource))
            return [];

        string newDestination = options.Rename!(normalizedDestination);
        if (normalizedSource != newDestination)
        {
            if (options.HookNames!.Contains(VFS.GetFileName(normalizedSource)))
                return [];
            NotificationProvider.Info("ntpc.copy", normalizedSource, newDestination);
            VFS.CopyFile(normalizedSource, newDestination);
            ntPcFiles.Add(
                new()
                {
                    Source = normalizedSource,
                    Destination = newDestination,
                    Crc = MaybeCreateCRC32(normalizedSource, true, options.CreateCRC32s),
                }
            );
        }

        return ntPcFiles;
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
