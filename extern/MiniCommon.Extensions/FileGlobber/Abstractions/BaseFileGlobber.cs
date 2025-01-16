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
using System.IO;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using MiniCommon.Extensions.FileGlobber.Interfaces;

namespace MiniCommon.Extensions.FileGlobber.Abstractions;

public class BaseFileGlobber(StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    : IBaseFileGlobber
{
    private Matcher _matcher = new(comparisonType);

    public Matcher Matcher
    {
        get => _matcher;
        private set
        {
            if (_matcher != value)
                _matcher = value;
        }
    }

    /// <inheritdoc/>
    public virtual PatternMatchingResult? MatchWithResult(string filepath) =>
        _matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(filepath)));

    /// <inheritdoc/>
    public virtual List<string> Match(string filepath) =>
        [.. _matcher.GetResultsInFullPath(filepath)];

    /// <inheritdoc/>
    public virtual void AddIncludePatterns(List<string> patterns)
    {
        if (patterns.Count != 0)
            _matcher.AddIncludePatterns(patterns);
    }

    /// <inheritdoc/>
    public virtual void AddExcludePatterns(List<string> patterns)
    {
        if (patterns.Count != 0)
            _matcher.AddExcludePatterns(patterns);
    }
}
