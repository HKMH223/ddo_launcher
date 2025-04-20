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
using MiniCommon.Managers.Abstractions;
using MiniCommon.Managers.Interfaces;

namespace MiniCommon.Managers;

public class CommandManager : ICommandManager
{
    public static BaseCommandManager Manager { get; } = new();

    /// <inheritdoc />
    public static async Task Initialize<T>(
        string[] args,
        List<IBaseCommand<T>> commands,
        T instance
    )
    {
        await Manager.Initialize(args, commands, instance);
    }
}
