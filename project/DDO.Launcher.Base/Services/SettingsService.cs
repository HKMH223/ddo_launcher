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
using DDO.Launcher.Base.Models;
using MiniCommon.Logger;
using MiniCommon.Managers;
using MiniCommon.Managers.Interfaces;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.Services;

public class SettingsService : IBaseService
{
    public static Settings? RuntimeSettings { get; set; }
    public static SettingsManager<Settings>? SettingsManager { get; private set; }

    public Task<bool> Initialize()
    {
        try
        {
            ServerInfo serverInfo = new()
            {
                ServerName = "Default",
                AccountAPI = "/api/account",
                DownloadIP = "127.0.0.1",
                DownloadPort = "52099",
                LobbyIP = "127.0.0.1",
                LobbyPort = "52100",
            };
            Settings settings = new()
            {
                Executable = "cwd:ddo.exe",
                ServerInfo = serverInfo,
                ServerInfos = [serverInfo],
                Account = "",
                Password = "",
                Email = "",
                RequireAdmin = false,
                LocalMode = false,
            };

            SettingsManager = new(SettingsContext.Default);
            SettingsManager.FirstRun(settings);
            RuntimeSettings = SettingsManager.Load();
            if (Validate.For.IsNull(RuntimeSettings))
                return Task.FromResult(false);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex.ToString());
            return Task.FromResult(false);
        }
    }
}
