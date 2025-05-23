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
using MiniCommon.Interfaces;
using MiniCommon.Logger;
using MiniCommon.Managers.Interfaces;

namespace MiniCommon.Managers.Abstractions;

public abstract class BaseCommandManager : IBaseCommandManager
{
    /// <inheritdoc />
    public virtual async Task Initialize(string[] args, List<IBaseCommand> commands)
    {
        try
        {
            foreach (IBaseCommand command in commands)
                await command.Initialize(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex.ToString());
        }
    }
}
