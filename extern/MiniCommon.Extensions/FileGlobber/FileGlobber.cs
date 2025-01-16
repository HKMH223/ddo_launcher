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
using Microsoft.Extensions.FileSystemGlobbing;
using MiniCommon.Extensions.FileGlobber.Abstractions;
using MiniCommon.Logger.Enums;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace MiniCommon.Extensions.FileGlobber;

public class FileGlobber(StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    : BaseFileGlobber(comparisonType)
{
    /// <inheritdoc/>
    public override PatternMatchingResult? Match(string filepath)
    {
        if (Validate.For.IsNullOrWhiteSpace([filepath], NativeLogLevel.Fatal))
            return null;
        return base.Match(filepath);
    }

    /// <inheritdoc/>
    public override void AddIncludePatterns(List<string> patterns)
    {
        if (Validate.For.IsNullOrEmpty(patterns, NativeLogLevel.Fatal))
            return;
        base.AddIncludePatterns(patterns);
    }

    /// <inheritdoc/>
    public override void AddExcludePatterns(List<string> patterns)
    {
        if (Validate.For.IsNullOrEmpty(patterns, NativeLogLevel.Fatal))
            return;
        base.AddExcludePatterns(patterns);
    }
}
