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
using DDO.Launcher.Base.Commands;
using DDO.Launcher.Base.Models;
using MiniCommon.CommandParser.Commands;
using MiniCommon.Interfaces;
using MiniCommon.Logger;

namespace DDO.Launcher.Base.Managers;

public static class CommandManager
{
    public static Settings? Settings { get; }

    /// <summary>
    /// Register a list of commands to be callable by the program.
    /// </summary>
    public static async Task Init(string[] args)
    {
        try
        {
            List<IBaseCommand<Settings>> commands = [];

            commands.Add(new Verifier());
            commands.Add(new Patcher());
            commands.Add(new Deploy());
            commands.Add(new Register());
            commands.Add(new Login());
            commands.Add(new Help<Settings>());

            foreach (IBaseCommand<Settings> command in commands)
                await command.Init(args, ServiceManager.Settings);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex.ToString());
            return;
        }
    }
}
