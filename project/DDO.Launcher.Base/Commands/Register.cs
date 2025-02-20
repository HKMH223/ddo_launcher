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
using DDO.Launcher.Base.Models;
using DDO.Launcher.Base.Services;
using MiniCommon.CommandParser;
using MiniCommon.Interfaces;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.Commands;

public class Register : IBaseCommand<Settings>
{
    /// <summary>
    /// Initiate a server regisration request through the client command line.
    /// Additional parameters take the format of 'key=value' pairs.
    /// </summary>
    public Task Init(string[] args, Settings? settings)
    {
        CommandLine.ProcessArgument(
            args,
            new()
            {
                Name = "register",
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
                ],
                Description = LocalizationProvider.Translate("command.register"),
            },
            options =>
            {
                if (Validate.For.IsNull(settings))
                    return;
                if (Validate.For.IsNull(settings!.ServerInfo))
                    return;

                settings!.ServerInfo!.AccountAPI = options.GetValueOrDefault("api", settings!.ServerInfo!.AccountAPI!);
                settings!.ServerInfo!.DownloadIP = options.GetValueOrDefault(
                    "download-ip",
                    settings!.ServerInfo!.DownloadIP!
                );
                settings!.ServerInfo!.DownloadPort = options.GetValueOrDefault(
                    "download-port",
                    settings!.ServerInfo!.DownloadPort!
                );
                settings!.ServerInfo!.LobbyIP = options.GetValueOrDefault("lobby-ip", settings!.ServerInfo!.LobbyIP!);
                settings!.ServerInfo!.LobbyPort = options.GetValueOrDefault(
                    "lobby-port",
                    settings!.ServerInfo!.LobbyPort!
                );
                settings!.Account = options.GetValueOrDefault("account", settings!.Account!);
                settings!.Password = options.GetValueOrDefault("password", settings!.Password!);
                settings!.Email = options.GetValueOrDefault("email", settings!.Email!);

                DDOAccountService service = new(settings);
                if (!service.Register())
                    return;
            }
        );

        return Task.CompletedTask;
    }
}
