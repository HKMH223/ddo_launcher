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
using System.Threading.Tasks;
using MiniCommon.BuildInfo;
using MiniCommon.Enums;
using MiniCommon.Logger;
using MiniCommon.Models;
using MiniCommon.Providers;

namespace DDO.ModManager.Base.Managers;

public static class ServiceManager
{
    /// <summary>
    /// Initialize required services and providers with necessary values.
    /// </summary>
    public static Task<bool> Init()
    {
        try
        {
            LocalizationProvider.Init(AssemblyConstants.LocalizationPath, Language.ENGLISH);
            NotificationProvider.OnNotificationAdded(
                (Notification notification) => Log.Base(notification.LogLevel, notification.Message)
            );
            NotificationProvider.Info("log.initialized");
            Watermark.Draw(AssemblyConstants.WatermarkText());
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex.ToString());
            return Task.FromResult(false);
        }
    }
}
