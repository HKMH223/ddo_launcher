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
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MiniCommon.IO;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace MiniCommon.Models;

public class Patch
{
    public int Offset { get; set; }
    public byte[]? Replacement { get; set; }
    public byte[]? Pattern { get; set; }

    /// <summary>
    /// Read and deserialize a file as a List<Patch> object.
    /// </summary>
    public static List<Patch> ReadPatchList(string path)
    {
        List<Patch>? rules = Json.Deserialize<List<Patch>>(
            VFS.ReadAllText(path),
            PatchContext.Default
        );

        if (Validate.For.IsNull(rules))
            return [];
        return rules!;
    }

    /// <summary>
    /// Read and deserialize a file as a Patch object.
    /// </summary>
    public static Patch? ReadPatchObject(string path)
    {
        Patch? rules = Json.Deserialize<Patch>(VFS.ReadAllText(path), PatchContext.Default);

        if (Validate.For.IsNull(rules))
            return null;
        return rules!;
    }

    /// <summary>
    /// Search through source bytes for pattern at a starting index.
    /// </summary>
    public static int FindPattern(byte[] source, byte[] pattern, int start)
    {
        for (int i = start; i <= source.Length - pattern.Length; i++)
        {
            if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Patch a byte buffer from a list of Patch objects.
    /// </summary>
    public static (int SuccessCount, byte[] PatchedBuffer) Exec(byte[] buffer, List<Patch> patches)
    {
        int counter = 0;

        for (int i = 0; i < patches.Count; i++)
        {
            if (Validate.For.IsNull(patches[i].Pattern))
                return (0, []);

            if (Validate.For.IsNull(patches[i].Replacement))
                return (0, []);

            int index = FindPattern(buffer, patches[i].Pattern!, 0);
            if (index == -1)
            {
                LogProvider.Warn(
                    "error.patch",
                    (i + 1).ToString(),
                    patches.Count.ToString(),
                    BitConverter.ToString(patches[i].Pattern!)
                );
            }
            else
            {
                Array.Copy(
                    patches[i].Replacement!,
                    0,
                    buffer,
                    index + patches[i].Offset,
                    patches[i].Replacement!.Length
                );
                LogProvider.Info(
                    "patch.success",
                    (i + 1).ToString(),
                    patches.Count.ToString(),
                    BitConverter.ToString(patches[i].Pattern!),
                    BitConverter.ToString(patches[i].Replacement!),
                    index + patches[i].Offset.ToString("X")
                );
                counter++;
            }
        }

        return (counter, buffer);
    }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(Patch))]
internal partial class PatchContext : JsonSerializerContext;
