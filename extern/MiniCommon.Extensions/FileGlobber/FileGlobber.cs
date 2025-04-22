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
    public override PatternMatchingResult? MatchWithResult(string filepath)
    {
        if (Validate.For.IsNullOrWhiteSpace([filepath], NativeLogLevel.Fatal))
            return null;
        return base.MatchWithResult(filepath);
    }

    /// <inheritdoc/>
    public override List<string> Match(string filepath)
    {
        if (Validate.For.IsNullOrWhiteSpace([filepath], NativeLogLevel.Fatal))
            return [];
        return base.Match(filepath);
    }

    /// <inheritdoc/>
    public override List<string> IncludePatterns
    {
#pragma warning disable S4275
        init
#pragma warning restore S4276
        {
            if (Validate.For.IsNullOrEmpty(value, NativeLogLevel.Fatal))
                return;
            base.IncludePatterns = value;
        }
    }

    /// <inheritdoc/>
    public override List<string> ExcludePatterns
    {
#pragma warning disable S4275
        init
#pragma warning restore S4276
        {
            if (Validate.For.IsNullOrEmpty(value, NativeLogLevel.Fatal))
                return;
            base.ExcludePatterns = value;
        }
    }

    /// <inheritdoc/>
    public override List<string> RegexIncludePatterns
    {
        get => base.RegexIncludePatterns;
#pragma warning disable S4275
        init
#pragma warning restore S4276
        {
            if (Validate.For.IsNullOrEmpty(value, NativeLogLevel.Fatal))
                return;
            base.RegexIncludePatterns = value;
        }
    }

    /// <inheritdoc/>
    public override List<string> RegexExcludePatterns
    {
        get => base.RegexExcludePatterns;
#pragma warning disable S4275
        init
#pragma warning restore S4276
        {
            if (Validate.For.IsNullOrEmpty(value, NativeLogLevel.Fatal))
                return;
            base.RegexExcludePatterns = value;
        }
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

    /// <inheritdoc/>
    public override void AddRegexIncludePatterns(List<string> patterns)
    {
        if (Validate.For.IsNullOrEmpty(patterns, NativeLogLevel.Fatal))
            return;
        base.AddRegexIncludePatterns(patterns);
    }

    /// <inheritdoc/>
    public override void AddRegexExcludePatterns(List<string> patterns)
    {
        if (Validate.For.IsNullOrEmpty(patterns, NativeLogLevel.Fatal))
            return;
        base.AddRegexExcludePatterns(patterns);
    }
}
