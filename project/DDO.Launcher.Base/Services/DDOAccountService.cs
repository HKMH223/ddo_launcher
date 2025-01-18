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

using DDO.Launcher.Base.Enums;
using DDO.Launcher.Base.Helpers;
using DDO.Launcher.Base.Models;
using DDO.Launcher.Base.Resolvers;
using MiniCommon.IO;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;
using MiniCommon.Web;
using MiniCommon.Web.Models;

namespace DDO.Launcher.Base.Services;

public class DDOAccountService
{
    public string? Token;
    private readonly Settings? _settings;

    private DDOAccountService() { }

    public DDOAccountService(Settings? settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Initiates a "login" request.
    /// </summary>
    public bool Login()
    {
        if (Validate.For.IsNull(_settings))
            return false;
        if (Validate.For.IsNull(_settings!.ServerInfo))
            return false;
        if (Validate.For.IsNullOrWhiteSpace([_settings!.Account, _settings.Password]))
            return false;
        if (
            Validate.For.IsNullOrWhiteSpace(
                [_settings!.ServerInfo!.AccountAPI, _settings.ServerInfo.DownloadIP, _settings.ServerInfo.DownloadPort]
            )
        )
        {
            return false;
        }
        return AccountRequest(ActionType.LOGIN);
    }

    /// <summary>
    /// Intitiates a "create" (register) request.
    /// </summary>
    public bool Register()
    {
        if (Validate.For.IsNull(_settings))
            return false;
        if (Validate.For.IsNull(_settings!.ServerInfo))
            return false;
        if (Validate.For.IsNullOrWhiteSpace([_settings!.Account, _settings.Password]))
            return false;
        if (
            Validate.For.IsNullOrWhiteSpace(
                [_settings!.ServerInfo!.AccountAPI, _settings.ServerInfo.DownloadIP, _settings.ServerInfo.DownloadPort]
            )
        )
        {
            return false;
        }
        return AccountRequest(ActionType.CREATE);
    }

    /// <summary>
    /// Execute a POST request using the TCP client.
    /// </summary>
    private bool AccountRequest(ActionType action)
    {
        if (_settings!.LocalMode == true)
        {
            NotificationProvider.Info("ddo.login.local");
            Token = LoginToken.Generate(_settings!.Account!, _settings.Password!);
            return true;
        }

        if (Tcp.EnsureConnection(_settings!.ServerInfo!.DownloadIP!, _settings.ServerInfo.DownloadPort!))
        {
            HttpRequest request = new()
            {
                Method = "POST",
                Version = "1.1",
                Path = _settings.ServerInfo.AccountAPI,
                Address = _settings.ServerInfo.DownloadIP,
                Port = _settings.ServerInfo.DownloadPort,
                ContentType = "application/json",
                Content = Json.Serialize(
                    new DDORequest()
                    {
                        Action = ActionTypeResolver.ToString(action),
                        Account = _settings?.Account,
                        Password = _settings?.Password,
                        Email = "",
                    },
                    DDORequestContext.Default
                ),
            };

            DDOResponse? response = Json.Deserialize<DDOResponse>(
                Tcp.Request(Tcp.ConstructHttpRequest(request)),
                DDOResponseContext.Default
            );
            if (Validate.For.IsNull(response))
                return false;

            if (response!.Error is not null)
            {
                if (response.Error == "Account already exists")
                {
                    NotificationProvider.Warn("tcp.error.response", response.Error);
                    return true;
                }

                NotificationProvider.Error("log.unhandled.exception", response.Error);
                return false;
            }

            if (response.Message == "Login Success")
                Token = response.Token;

            return true;
        }

        return false;
    }
}
