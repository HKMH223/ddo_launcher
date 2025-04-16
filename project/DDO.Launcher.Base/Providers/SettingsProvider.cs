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

using DDO.Launcher.Base.Models;
using MiniCommon.BuildInfo;
using MiniCommon.IO;
using MiniCommon.IO.Helpers;
using MiniCommon.Providers;

namespace DDO.Launcher.Base.Providers;

public static class SettingsProvider
{
    public const string SettingsFileName = "settings.json";
    private static readonly string _settingsFilePath = VFS.FromCwd(AssemblyConstants.DataDirectory, SettingsFileName);

    /// <summary>
    /// Initialize the Settings service.
    /// Create a new configuration file if one is not present.
    /// </summary>
    public static void FirstRun()
    {
        (string? resultSettingsFilePath, string expectedSettingsFilePath) = JsonExtensionHelper.MaybeJsonWithComments(
            _settingsFilePath
        );
        if (resultSettingsFilePath is null)
        {
            NotificationProvider.Warn("launcher.settings.missing", SettingsFileName, AssemblyConstants.DataDirectory);
            NotificationProvider.Info("launcher.settings.setup", expectedSettingsFilePath);
            ServerInfo serverInfo = new()
            {
                ServerName = "Default",
                AccountAPI = "/api/account",
                DownloadIP = "127.0.0.1",
                DownloadPort = "52099",
                LobbyIP = "127.0.0.1",
                LobbyPort = "52100",
            };
            Settings settings = new()
            {
                Executable = "cwd:ddo.exe",
                ServerInfo = serverInfo,
                ServerInfos = [serverInfo],
                Account = "",
                Password = "",
                Email = "",
                RequireAdmin = false,
                LocalMode = false,
            };

            Json.Save(expectedSettingsFilePath, settings, SettingsContext.Default);
        }

        NotificationProvider.Info("launcher.settings.using", expectedSettingsFilePath);
    }

    /// <summary>
    /// Save a Settings object to the Settings file path.
    /// </summary>
    public static void Save(Settings settings)
    {
        (string? resultSettingsFilePath, string expectedSettingsFilePath) = JsonExtensionHelper.MaybeJsonWithComments(
            _settingsFilePath
        );

        NotificationProvider.Info("launcher.settings.save", expectedSettingsFilePath);

        if (resultSettingsFilePath is null)
            return;

        Settings? existingSettings = Load();
        if (existingSettings == settings)
            return;

        Json.Save(expectedSettingsFilePath, settings, SettingsContext.Default);
    }

    /// <summary>
    /// Load a Settings object from the Settings file path.
    /// </summary>
    public static Settings? Load()
    {
        (string? resultSettingsFilePath, string expectedSettingsFilePath) = JsonExtensionHelper.MaybeJsonWithComments(
            _settingsFilePath
        );

        NotificationProvider.Info("launcher.settings.load", expectedSettingsFilePath);

        if (resultSettingsFilePath is not null)
        {
            Settings? inputJson = Json.Load<Settings>(resultSettingsFilePath!, SettingsContext.Default);
            if (inputJson is not null)
                return inputJson;

            NotificationProvider.Error("launcher.settings.missing", SettingsFileName, AssemblyConstants.DataDirectory);
            return null;
        }

        NotificationProvider.Error("launcher.settings.missing", SettingsFileName, AssemblyConstants.DataDirectory);
        return null;
    }
}
