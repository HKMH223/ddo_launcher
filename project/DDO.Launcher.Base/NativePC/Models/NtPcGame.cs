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
using MiniCommon.IO;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.NativePC.Models;

public class NtPcGame
{
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("Formats")]
    public List<string>? Formats { get; set; }

    [JsonPropertyName("Engine")]
    public NtPcEngine? Engine { get; set; }

    public NtPcGame() { }

    /// <summary>
    /// Read and deserialize a file as an NtPcGame object.
    /// </summary>
    public static NtPcGame Read(string path)
    {
        NtPcGame? game = Json.Deserialize<NtPcGame>(VFS.ReadAllText(path), NtPcGameContext.Default);

        if (Validate.For.IsNull(game))
            return new();
        return game!;
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(NtPcGame))]
internal partial class NtPcGameContext : JsonSerializerContext;
