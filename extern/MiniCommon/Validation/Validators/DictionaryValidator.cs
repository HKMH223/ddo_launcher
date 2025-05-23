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
using System.Runtime.CompilerServices;
using MiniCommon.Logger.Enums;
using MiniCommon.Providers;
using MiniCommon.Validation.Abstractions;
using MiniCommon.Validation.Interfaces;
using MiniCommon.Validation.Operators;

namespace MiniCommon.Validation.Validators;

#pragma warning disable IDE0060, RCS1175, RCS1163, RCS1158, S107

public static class DictionaryValidator
{
    /// <summary>
    /// Validate a dictionary is not null, or empty.
    /// </summary>
    public static bool IsNotNullOrEmpty<T1, T2>(
        this IValidationClause clause,
        Dictionary<T1, T2>? obj,
        NativeLogLevel level = NativeLogLevel.Error,
        Dictionary<T1, T2>[]? properties = null,
        [CallerArgumentExpression(nameof(obj))] string objName = "",
        [CallerArgumentExpression(nameof(properties))] string propertiesName = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
        where T1 : notnull =>
        !IsNullOrEmpty(
            clause,
            obj,
            string.Empty,
            level,
            properties,
            objName,
            propertiesName,
            memberName,
            sourceFilePath,
            sourceLineNumber
        );

    /// <summary>
    /// Validate a dictionary is not null, or empty.
    /// </summary>
    public static bool IsNotNullOrEmpty<T1, T2>(
        this IValidationClause clause,
        Dictionary<T1, T2>? obj,
        string message,
        NativeLogLevel level = NativeLogLevel.Error,
        Dictionary<T1, T2>[]? properties = null,
        [CallerArgumentExpression(nameof(obj))] string objName = "",
        [CallerArgumentExpression(nameof(properties))] string propertiesName = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
        where T1 : notnull =>
        !IsNullOrEmpty(
            clause,
            obj,
            message,
            level,
            properties,
            objName,
            propertiesName,
            memberName,
            sourceFilePath,
            sourceLineNumber
        );

    /// <summary>
    /// Validate a dictionary is null, or empty.
    /// </summary>
    public static bool IsNullOrEmpty<T1, T2>(
        this IValidationClause clause,
        Dictionary<T1, T2>? obj,
        NativeLogLevel level = NativeLogLevel.Error,
        Dictionary<T1, T2>[]? properties = null,
        [CallerArgumentExpression(nameof(obj))] string objName = "",
        [CallerArgumentExpression(nameof(properties))] string propertiesName = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
        where T1 : notnull =>
        IsNullOrEmpty(
            clause,
            obj,
            string.Empty,
            level,
            properties,
            objName,
            propertiesName,
            memberName,
            sourceFilePath,
            sourceLineNumber
        );

    /// <summary>
    /// Validate a dictionary is null, or empty.
    /// </summary>
    public static bool IsNullOrEmpty<T1, T2>(
        this IValidationClause clause,
        Dictionary<T1, T2>? obj,
        string message,
        NativeLogLevel level = NativeLogLevel.Error,
        Dictionary<T1, T2>[]? properties = null,
        [CallerArgumentExpression(nameof(obj))] string objName = "",
        [CallerArgumentExpression(nameof(properties))] string propertiesName = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
        where T1 : notnull
    {
        BaseValidator<Dictionary<T1, T2>> validator = new();

        if (string.IsNullOrWhiteSpace(message))
        {
            message = LocalizationProvider.FormatTranslate(
                "error.validation.list",
                objName,
                propertiesName,
                memberName,
                sourceFilePath,
                sourceLineNumber.ToString()
            );
        }
        else
        {
            message = string.Format(
                message,
                objName ?? Validate.For.EmptyString(),
                propertiesName ?? Validate.For.EmptyString(),
                memberName,
                sourceFilePath,
                sourceLineNumber.ToString()
            );
        }
        validator.AddRule(a => obj?.Count > 0, message);
        foreach (
            Dictionary<T1, T2> property in properties
                ?? [.. Validate.For.EmptyList<Dictionary<T1, T2>>()]
        )
        {
            validator.AddRule(a => property?.Count > 0, message);
        }

        return !validator.Validate(obj, level);
    }
}
