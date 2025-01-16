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
using DDO.Launcher.Base.Helpers;
using DDO.Launcher.Base.Models;
using DDO.Launcher.Base.Services;
using MiniCommon.CommandParser;
using MiniCommon.Interfaces;
using MiniCommon.IO;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.Commands;

public class Login : IBaseCommand<Settings>
{
    /// <summary>
    /// Initiate a server login request through the client command line.
    /// Additional parameters take the format of 'key=value' pairs.
    /// </summary>
    public Task Init(string[] args, Settings? settings)
    {
        CommandLine.ProcessArgument(
            args,
            new()
            {
                Name = "login",
                Parameters =
                [
                    new() { Name = "api", Optional = true },
                    new() { Name = "download-ip", Optional = true },
                    new() { Name = "download-port", Optional = true },
                    new() { Name = "lobby-ip", Optional = true },
                    new() { Name = "lobby-port", Optional = true },
                    new() { Name = "account", Optional = true },
                    new() { Name = "password", Optional = true },
                    new() { Name = "email", Optional = true },
                    new() { Name = "launch", Optional = false },
                ],
                Description = LocalizationProvider.Translate("command.login"),
            },
            options =>
            {
                if (Validate.For.IsNull(settings))
                    return;

                settings!.AccountAPI = options.GetValueOrDefault("api", "/api/account");
                settings!.DownloadIP = options.GetValueOrDefault("download-ip", "127.0.0.1");
                settings!.DownloadPort = options.GetValueOrDefault("download-port", "52099");
                settings!.LobbyIP = options.GetValueOrDefault("lobby-ip", "127.0.0.1");
                settings!.LobbyPort = options.GetValueOrDefault("lobby-port", "52100");
                settings!.Account = options.GetValueOrDefault("account", settings!.Account!);
                settings!.Password = options.GetValueOrDefault("password", settings!.Password!);
                settings!.Email = options.GetValueOrDefault("email", settings!.Email!);

                if (!bool.TryParse(options.GetValueOrDefault("launch", "true"), out bool launch))
                    return;

                DDOAccountService service = new(settings);
                if (!service.Login())
                    return;

                if (launch)
                    DDOLauncher.Launch(settings!, service.Token ?? Validate.For.EmptyString(), VFS.FileSystem.Cwd);
            }
        );

        return Task.CompletedTask;
    }
}
