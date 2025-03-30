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
using System.Text.Json.Serialization;

namespace DDO.ModManager.Base.NativePC.Helpers.Models;

public class CopyOptions
{
    [JsonPropertyName("Source")]
    public string? Source { get; set; }

    [JsonPropertyName("Destination")]
    public string? Destination { get; set; }

    [JsonPropertyName("SearchDirectory")]
    public string? SearchDirectory { get; set; }

    [JsonPropertyName("Skip")]
    public Func<string, bool>? Skip { get; set; }

    [JsonPropertyName("Rename")]
    public Func<string, string>? Rename { get; set; }

    [JsonPropertyName("IgnorePrefixes")]
    public string[]? IgnorePrefixes { get; set; }

    [JsonPropertyName("HookNames")]
    public List<string>? HookNames { get; set; }

    [JsonPropertyName("CreateCRC32s")]
    public bool CreateCRC32s { get; set; }

    public CopyOptions() { }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(CopyOptions))]
internal partial class CopyOptionsContext : JsonSerializerContext;
