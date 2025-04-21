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
using Avalonia;
using DDO.Launcher.Base;
using DDO.Launcher.Base.Commands;
using DDO.ModManager.Base.Commands;
using MiniCommon.BuildInfo;
using MiniCommon.CommandParser.Commands;
using MiniCommon.IO;
using MiniCommon.Logger;
using MiniCommon.Logger.Enums;

namespace DDO.Launcher;

static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task Main(string[] args)
    {
        // Call necessary functions before starting the Avalonia application.
        AssemblyConstants.AssemblyName = "DDO.Launcher";
        AssemblyConstants.DataDirectory = ".ddo_launcher";

        VFS.FileSystem.Cwd = AppDomain.CurrentDomain.BaseDirectory;

        Log.Add(new NativeLogger(NativeLogLevel.Info, CensorLevel.REDACT));
        Log.Add(new FileStreamLogger(AssemblyConstants.LogFilePath(), NativeLogLevel.Info, CensorLevel.REDACT));
        await RuntimeManager.Initialize(
            args,
            [
                new Verifier(RuntimeManager.RuntimeSettings!),
                new Patcher(RuntimeManager.RuntimeSettings!),
                new Deploy(),
                new Register(RuntimeManager.RuntimeSettings!),
                new Login(RuntimeManager.RuntimeSettings!),
                new Help(),
            ]
        );

        if (args.Length != 0)
            return;

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
}
