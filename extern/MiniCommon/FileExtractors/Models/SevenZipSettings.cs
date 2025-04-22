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
using MiniCommon.Models;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace MiniCommon.FileExtractors.Models;

public class SevenZipSettings
{
    public string Executable { get; set; } = "7z";

    public bool UseShellExecute { get; set; }

    public bool Silent { get; set; }

    public static MOption[]? DefaultExtractArguments(string source, string destination)
    {
        if (Validate.For.IsNullOrWhiteSpace([source, destination]))
            return null;

        return
        [
            new MOption { Arg = "x \"{0}\"", ArgParams = [source] },
            new MOption { Arg = "-o\"{0}\"/*", ArgParams = [destination] },
            new MOption { Arg = "-y" },
        ];
    }

    public static MOption[]? DefaultCompressionArguments(string source, string destination)
    {
        if (Validate.For.IsNullOrWhiteSpace([source, destination]))
            return null;

        return
        [
            new MOption { Arg = "a" },
            new MOption { Arg = "-t{0} {1} {2}", ArgParams = ["7z", destination, source + "/*"] },
            new MOption { Arg = "-mx{0}", ArgParams = ["9"] },
            new MOption { Arg = "-m0={0}", ArgParams = ["lzma2"] },
            new MOption { Arg = "-md={0}", ArgParams = ["64m"] },
            new MOption { Arg = "-mfb={0}", ArgParams = ["64"] },
            new MOption { Arg = "-ms={0}", ArgParams = ["4g"] },
            new MOption { Arg = "-mmt={0}", ArgParams = ["2"] },
            new MOption { Arg = "-mmemuse={0}", ArgParams = ["26g"] },
        ];
    }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(SevenZipSettings))]
internal partial class SevenZipSettingsContext : JsonSerializerContext;
