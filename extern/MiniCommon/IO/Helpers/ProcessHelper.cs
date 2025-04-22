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
using System.Diagnostics;
using MiniCommon.Logger.Enums;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace MiniCommon.IO.Helpers;

public static class ProcessHelper
{
    /// <summary>
    /// Create and run a new process.
    /// </summary>
#pragma warning disable S3776
    public static void RunProcess(
        string fileName,
        string arguments,
        string workingDirectory,
        bool useShellExecute = true,
        bool silent = false,
        string verb = "",
        Dictionary<string, string>? environmentalVariables = null
    )
#pragma warning restore S3776
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            UseShellExecute = useShellExecute,
            RedirectStandardOutput = !useShellExecute,
            RedirectStandardError = !useShellExecute,
            Verb = verb,
        };

        if (environmentalVariables?.Count > 0)
        {
            foreach ((string name, string item) in environmentalVariables)
                startInfo.EnvironmentVariables[name] = item;
        }

        try
        {
            Process process = new() { StartInfo = startInfo };

            if (!useShellExecute)
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (silent)
                        return;
                    if (Validate.For.IsNotNullOrWhiteSpace([e?.Data], NativeLogLevel.Debug))
                        LogProvider.PrintLog(DetermineLogType(e!.Data!), e!.Data!);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (silent)
                        return;
                    if (Validate.For.IsNotNullOrWhiteSpace([e?.Data], NativeLogLevel.Debug))
                        LogProvider.PrintLog(DetermineLogType(e!.Data!), e!.Data!);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            else
            {
                process.Start();
                if (!silent)
                    LogProvider.InfoLog(process.StandardOutput.ReadToEnd());
            }

            process.WaitForExit();
        }
        catch (Exception ex)
        {
            LogProvider.Error(
                "log.stack.trace",
                ex.Message,
                ex.StackTrace ?? LocalizationProvider.Translate("stack.trace.null")
            );
        }
    }

    /// <summary>
    /// Check if a data string contains a log related substring.
    /// </summary>
    private static NativeLogLevel DetermineLogType(string data)
    {
        if (data.Contains("WARN", StringComparison.CurrentCultureIgnoreCase))
            return NativeLogLevel.Warn;
        else if (data.Contains("ERROR", StringComparison.CurrentCultureIgnoreCase))
            return NativeLogLevel.Error;
        return NativeLogLevel.Info;
    }
}
