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
using DDO.ModManager.Base;
using DDO.ModManager.Base.Commands;
using MiniCommon.BuildInfo;
using MiniCommon.CommandParser.Commands;
using MiniCommon.CommandParser.Helpers;
using MiniCommon.IO;
using MiniCommon.Logger;
using MiniCommon.Logger.Enums;
using MiniCommon.Models;
using MiniCommon.Providers;

namespace DDO.ModManager.Cli;

static class Program
{
    public static async Task Main(string[] args)
    {
        AssemblyConstants.AssemblyName = "DDO.ModManager.Cli";
        AssemblyConstants.SettingsFileName = "modding.settings.json";
        AssemblyConstants.DataDirectory = ".ddo_launcher";

        VFS.FileSystem.Cwd = AppDomain.CurrentDomain.BaseDirectory;

        Log.Add(new NativeLogger(NativeLogLevel.Info, CensorLevel.REDACT));
        Log.Add(new FileStreamLogger(AssemblyConstants.LogFilePath(), NativeLogLevel.Info, CensorLevel.REDACT));
        await RuntimeManager.Initialize(
            args,
            [_ => new Deploy(), settings => new RunFileViewer(settings), _ => new Help()]
        );

        if (args.Length == 0)
        {
            foreach (Command command in CommandHelper.Commands)
                LogProvider.InfoLog(command.Usage());
        }

        return;
    }
}
