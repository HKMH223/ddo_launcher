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
using MiniCommon.IO;
using MiniCommon.IO.Helpers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace MiniCommon.Models;

public class LocalizationSettings
{
    [JsonPropertyName("Language")]
    public string? Language { get; set; } = "en";

    [JsonIgnore]
    public static readonly string? DefaultLanguageCode = "en";

    /// <summary>
    /// Read and deserialize a file as a LocalizationSettings object, and return the language name.
    /// </summary>
    public static string Read(string path)
    {
        (string? result, _) = JsonExtensionHelper.MaybeJsonWithComments(path);
        if (Validate.For.IsNullOrWhiteSpace([path, result]))
            return DefaultLanguageCode!;

        LocalizationSettings? settings = Json.Deserialize<LocalizationSettings>(
            VFS.ReadAllText(result!),
            LocalizationSettingsContext.Default
        );

        if (Validate.For.IsNull(settings))
            return DefaultLanguageCode!;
        if (Validate.For.IsNullOrWhiteSpace([settings!.Language]))
            return DefaultLanguageCode!;
        return settings!.Language!;
    }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(LocalizationSettings))]
internal partial class LocalizationSettingsContext : JsonSerializerContext;
