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

using System.Text.Json.Serialization;
using MiniCommon.BuildInfo;
using MiniCommon.IO;
using MiniCommon.IO.Helpers;
using MiniCommon.Managers.Interfaces;
using MiniCommon.Providers;

namespace MiniCommon.Managers.Abstractions;

public abstract class BaseSettingsManager<T>(JsonSerializerContext _ctx) : IBaseSettingsManager<T>
    where T : class
{
    private JsonSerializerContext SerializerContext { get; } = _ctx;

    /// <inheritdoc />
    public virtual void FirstRun(T data)
    {
        (string? resultSettingsFilePath, string expectedSettingsFilePath) =
            JsonExtensionHelper.MaybeJsonWithComments(AssemblyConstants.SettingsFilePath());
        if (resultSettingsFilePath is null)
        {
            LogProvider.Warn(
                "launcher.settings.missing",
                AssemblyConstants.SettingsFileName,
                AssemblyConstants.DataDirectory
            );
            LogProvider.Info("launcher.settings.setup", expectedSettingsFilePath);
            Json.Save(expectedSettingsFilePath, data, SerializerContext);
        }

        LogProvider.Info("launcher.settings.using", expectedSettingsFilePath);
    }

    /// <inheritdoc />
    public virtual void Save(T data)
    {
        (string? resultSettingsFilePath, string expectedSettingsFilePath) =
            JsonExtensionHelper.MaybeJsonWithComments(AssemblyConstants.SettingsFilePath());

        LogProvider.Info("launcher.settings.save", expectedSettingsFilePath);

        if (resultSettingsFilePath is null)
            return;

        T? existingSettings = Load();
        if (existingSettings!.Equals(data))
            return;

        Json.Save(expectedSettingsFilePath, data, SerializerContext);
    }

    /// <inheritdoc />
    public virtual T? Load()
    {
        (string? resultSettingsFilePath, string expectedSettingsFilePath) =
            JsonExtensionHelper.MaybeJsonWithComments(AssemblyConstants.SettingsFilePath());

        LogProvider.Info("launcher.settings.load", expectedSettingsFilePath);

        if (resultSettingsFilePath is not null)
        {
            T? inputJson = Json.Load<T>(resultSettingsFilePath!, SerializerContext);
            if (inputJson is not null)
                return inputJson;

            LogProvider.Error(
                "launcher.settings.missing",
                AssemblyConstants.SettingsFileName,
                AssemblyConstants.DataDirectory
            );
            return default;
        }

        LogProvider.Error(
            "launcher.settings.missing",
            AssemblyConstants.SettingsFileName,
            AssemblyConstants.DataDirectory
        );
        return default;
    }
}
