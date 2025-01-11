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

using System.Text.Json.Serialization;

namespace DDO.Launcher.Base.Models;

public class Settings
{
    public string? Executable { get; set; }

    public string? AccountAPI { get; set; }
    public string? DownloadIP { get; set; }
    public string? DownloadPort { get; set; }
    public string? LobbyIP { get; set; }
    public string? LobbyPort { get; set; }

    public string? Account { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }

    public bool? RequireAdmin { get; set; }
    public bool? LocalMode { get; set; }

    public Settings() { }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Settings))]
internal partial class SettingsContext : JsonSerializerContext;
