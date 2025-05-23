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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace DDO.Launcher.Base.Models;

#pragma warning disable S101
public class DDORequest
#pragma warning restore S101
{
    [JsonPropertyName("Action")]
    public string? Action { get; set; }

    [JsonPropertyName("Account")]
    public string? Account { get; set; }

    [JsonPropertyName("Password")]
    public string? Password { get; set; }

    [JsonPropertyName("Email")]
    public string? Email { get; set; }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(DDORequest))]
internal partial class DDORequestContext : JsonSerializerContext;
