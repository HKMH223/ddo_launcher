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
using System.Threading.Tasks;
using DDO.Launcher.Base.Models;
using DDO.Launcher.Base.Services;
using MiniCommon.Interfaces;
using MiniCommon.Managers;
using MiniCommon.Managers.Interfaces;
using MiniCommon.Managers.Services;

namespace DDO.Launcher.Base;

public class RuntimeManager : IRuntimeManager<Settings>
{
    public static Settings? RuntimeSettings { get; private set; }
    public static SettingsManager<Settings>? SettingsManager { get; private set; }

    public static async Task<bool> Initialize(string[] args, List<Func<Settings, IBaseCommand>> commandFactories)
    {
        SettingsService service = new();
        bool result = await ServiceManager.Initialize(
            [
                new LocalizationService(),
                new LogService(),
                service,
                new RequestService(),
                new TcpService(),
                new WatermarkService(),
            ]
        );

        RuntimeSettings = SettingsService.RuntimeSettings;
        SettingsManager = SettingsService.SettingsManager;

        if (args.Length != 0 && RuntimeSettings is not null)
        {
            List<IBaseCommand> commands = commandFactories.ConvertAll(f => f(RuntimeSettings));
            await CommandManager.Initialize(args, commands);
        }

        return result;
    }
}
