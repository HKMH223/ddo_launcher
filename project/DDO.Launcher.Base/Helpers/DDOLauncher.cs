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

using DDO.Launcher.Base.Models;
using MiniCommon.Enums;
using MiniCommon.Extensions;
using MiniCommon.IO.Helpers;
using MiniCommon.Providers;
using MiniCommon.Resolvers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.Helpers;

#pragma warning disable S101
public static class DDOLauncher
#pragma warning restore S101
{
    /// <summary>
    /// Launch the game process.
    /// </summary>
    public static void Launch(Settings runtimeSettings, string token, string workingDirectory)
    {
        if (
            Validate.For.IsNull(runtimeSettings)
            || Validate.For.IsNullOrWhiteSpace([runtimeSettings.Executable])
            || Validate.For.IsNullOrWhiteSpace([workingDirectory])
        )
        {
            return;
        }

        if (!WindowsAdminHelper.IsAdmin() && runtimeSettings.RequireAdmin == true)
        {
            LogProvider.Error("error.admin.required");
            return;
        }

        LogProvider.InfoLog(
            MLaunchOptions.DefaultArguments(runtimeSettings!, token!)?.Arguments()?.Build()
                ?? Validate.For.EmptyString()
        );

        ProcessHelper.RunProcess(
            PathHelper.MaybeCwd(runtimeSettings.Executable!, workingDirectory),
            MLaunchOptions.DefaultArguments(runtimeSettings!, token!)?.Arguments()?.Build()
                ?? Validate.For.EmptyString(),
            workingDirectory,
            true,
            false,
            runtimeSettings.RequireAdmin == true
                ? ProcessActionResolver.ToString(ProcessAction.OPEN)
                : ProcessActionResolver.ToString(ProcessAction.RUNAS)
        );
    }
}
