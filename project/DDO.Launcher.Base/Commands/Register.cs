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
                    new() { Name = "dip", Optional = true },
                    new() { Name = "dport", Optional = true },
                    new() { Name = "lip", Optional = true },
                    new() { Name = "lport", Optional = true },
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

                settings!.AccountAPI = options.GetValueOrDefault("api", "/api/account");
                settings!.DownloadIP = options.GetValueOrDefault("dip", "127.0.0.1");
                settings!.DownloadPort = options.GetValueOrDefault("dport", "52099");
                settings!.LobbyIP = options.GetValueOrDefault("lip", "127.0.0.1");
                settings!.LobbyPort = options.GetValueOrDefault("lport", "52100");
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
