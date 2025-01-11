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
using System.Runtime.InteropServices;
using System.Security.Principal;
using MiniCommon.Providers;

namespace MiniCommon.IO.Helpers;

public static class WindowsAdminHelper
{
    public static bool IsAdmin()
    {
        bool result = false;
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                result = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(
                    WindowsBuiltInRole.Administrator
                );
            }
        }
        catch (Exception ex)
        {
            NotificationProvider.Error(
                "log.stack.trace",
                ex.Message,
                ex.StackTrace ?? LocalizationProvider.Translate("stack.trace.null")
            );
        }
        return result;
    }
}
