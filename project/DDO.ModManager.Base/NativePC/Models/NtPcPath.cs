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

namespace DDO.ModManager.Base.NativePC.Models;

public class NtPcPath
{
    [JsonPropertyName("Path")]
    public string? Path { get; set; }

    [JsonPropertyName("IgnoreCase")]
    public bool IgnoreCase { get; set; }

    [JsonPropertyName("IsDir")]
    public bool? IsDir { get; set; }

    [JsonPropertyName("Requires")]
    public List<string>? Requires { get; set; }

    [JsonPropertyName("Unsupported")]
    public bool? Unsupported { get; set; }

    [JsonPropertyName("FromDirIndex")]
    public int FromDirIndex { get; set; }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(NtPcPath))]
internal partial class NtPcPathContext : JsonSerializerContext;
