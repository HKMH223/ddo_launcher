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
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using MiniCommon.Cryptography.Helpers;
using MiniCommon.Extensions.FileGlobber.Interfaces;

namespace MiniCommon.Extensions.FileGlobber.Abstractions;

public class BaseFileGlobber(StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    : IBaseFileGlobber
{
    private Matcher _matcher = new(comparisonType);

    private readonly List<string> _includePatterns = [];
    private readonly List<string> _excludePatterns = [];

    protected List<string> _regexIncludePatterns = [];
    protected List<string> _regexExcludePatterns = [];

    protected List<Regex> _compiledRegexIncludes = [];
    protected List<Regex> _compiledRegexExcludes = [];

    private int _lastRegexIncludeHash = 0;
    private int _lastRegexExcludeHash = 0;

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
    public virtual List<string> Match(string filepath)
    {
        List<string> matches = [.. _matcher.GetResultsInFullPath(filepath)];
        CompileRegexPatterns();
        return matches
            .Where(file =>
            {
                string fileName = Path.GetFileName(file);
                bool includeMatch =
                    _compiledRegexIncludes.Count == 0
                    || _compiledRegexIncludes.Any(r => r.IsMatch(fileName));
                bool excludeMatch = _compiledRegexExcludes.Any(r => r.IsMatch(fileName));
                return includeMatch && !excludeMatch;
            })
            .ToList();
    }

    /// <inheritdoc/>
    public virtual List<string> IncludePatterns
    {
        get => _includePatterns;
        init => AddIncludePatterns(value.Distinct().ToList());
    }

    /// <inheritdoc/>
    public virtual List<string> ExcludePatterns
    {
        get => _excludePatterns;
        init => AddExcludePatterns(value.Distinct().ToList());
    }

    /// <inheritdoc/>
    public virtual List<string> RegexIncludePatterns
    {
        get => _regexIncludePatterns;
        init
        {
            if (_regexIncludePatterns.SequenceEqual(value))
                return;

            _regexIncludePatterns = value;
            CompileRegexPatterns();
        }
    }

    /// <inheritdoc/>
    public virtual List<string> RegexExcludePatterns
    {
        get => _regexExcludePatterns;
        init
        {
            if (_regexExcludePatterns.SequenceEqual(value))
                return;

            _regexExcludePatterns = value;
            CompileRegexPatterns();
        }
    }

    /// <inheritdoc/>
    public virtual void AddIncludePatterns(List<string> patterns)
    {
        if (patterns.Count == 0)
            return;

        List<string> newPatterns = patterns.Except(_includePatterns).ToList();
        _includePatterns.AddRange(newPatterns);
        _matcher.AddIncludePatterns(newPatterns);
    }

    /// <inheritdoc/>
    public virtual void AddExcludePatterns(List<string> patterns)
    {
        if (patterns.Count == 0)
            return;

        List<string> newPatterns = patterns.Except(_excludePatterns).ToList();
        _excludePatterns.AddRange(newPatterns);
        _matcher.AddExcludePatterns(newPatterns);
    }

    /// <inheritdoc/>
    public virtual void AddRegexIncludePatterns(List<string> patterns)
    {
        if (patterns.Count != 0)
        {
            if (_regexIncludePatterns.SequenceEqual(patterns))
                return;

            _regexIncludePatterns = [.. _regexIncludePatterns, .. patterns];
            _lastRegexIncludeHash = 0;
            CompileRegexPatterns();
        }
    }

    /// <inheritdoc/>
    public virtual void AddRegexExcludePatterns(List<string> patterns)
    {
        if (patterns.Count != 0)
        {
            if (_regexExcludePatterns.SequenceEqual(patterns))
                return;

            _regexExcludePatterns = [.. _regexExcludePatterns, .. patterns];
            _lastRegexExcludeHash = 0;
            CompileRegexPatterns();
        }
    }

    /// <inheritdoc/>
    public virtual void CompileRegexPatterns()
    {
        int currentIncludeHash = CryptographyHelper.ComputeHash(_regexIncludePatterns);
        int currentExcludeHash = CryptographyHelper.ComputeHash(_regexExcludePatterns);

        if (
            currentIncludeHash == _lastRegexIncludeHash
            && currentExcludeHash == _lastRegexExcludeHash
        )
            return;

        _compiledRegexIncludes =
        [
            .. _regexIncludePatterns.Select(p => new Regex(p, RegexOptions.IgnoreCase)),
        ];
        _compiledRegexExcludes =
        [
            .. _regexExcludePatterns.Select(p => new Regex(p, RegexOptions.IgnoreCase)),
        ];

        _lastRegexIncludeHash = currentIncludeHash;
        _lastRegexExcludeHash = currentExcludeHash;
    }
}
