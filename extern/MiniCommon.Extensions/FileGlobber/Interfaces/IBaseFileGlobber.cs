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
using Microsoft.Extensions.FileSystemGlobbing;

namespace MiniCommon.Extensions.FileGlobber.Interfaces;

public interface IBaseFileGlobber
{
    /// <summary>
    /// Glob files from a specific search directory.
    /// </summary>
    public abstract PatternMatchingResult? MatchWithResult(string filepath);

    /// <summary>
    /// Glob files from a specific search directory.
    /// </summary>
    public abstract List<string> Match(string filepath);

    /// <summary>
    /// Add include patterns to the file matcher.
    /// </summary>
    public abstract List<string> IncludePatterns { init; }

    /// <summary>
    /// Add exclude patterns to the file matcher.
    /// </summary>
    public abstract List<string> ExcludePatterns { init; }

    /// <summary>
    /// Add regex include patterns to the file matcher.
    /// </summary>
    public abstract List<string> RegexIncludePatterns { init; }

    /// <summary>
    /// Add regex exclude patterns to the file matcher.
    /// </summary>
    public abstract List<string> RegexExcludePatterns { init; }

    /// <summary>
    /// Add include patterns to the file matcher.
    /// </summary>
    public abstract void AddIncludePatterns(List<string> patterns);

    /// <summary>
    /// Add exclude patterns to the file matcher.
    /// </summary>
    public abstract void AddExcludePatterns(List<string> patterns);

    /// <summary>
    /// Add regex include patterns to the file matcher.
    /// </summary>
    public abstract void AddRegexIncludePatterns(List<string> patterns);

    /// <summary>
    /// Add regex exclude patterns to the file matcher.
    /// </summary>
    public abstract void AddRegexExcludePatterns(List<string> patterns);

    /// <summary>
    /// Compile Regex patterns used by RegexIncludePatterns() and RegexExcludePatterns().
    /// </summary>
    public abstract void CompileRegexPatterns();
}
