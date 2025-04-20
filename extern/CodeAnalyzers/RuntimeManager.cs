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
using System.Threading.Tasks;
using MiniCommon.Interfaces;
using MiniCommon.Managers.Interfaces;
using MiniCommon.Managers.Services;
using MManager = MiniCommon.Managers;

namespace CodeAnalyzers;

public class RuntimeManager : IRuntimeManager<object>
{
    public static async Task<bool> Initialize(string[] args, List<IBaseCommand<object>> commands)
    {
        bool result = await MManager.ServiceManager.Initialize(
            [new LocalizationService(), new NotificationService(), new WatermarkService()],
            new object()
        );
        if (args.Length != 0)
            await MManager.CommandManager.Initialize(args, commands, new object());
        return result;
    }
}
