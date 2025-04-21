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
using MiniCommon.Managers.Abstractions;
using MiniCommon.Managers.Interfaces;

namespace MiniCommon.Managers;

public class SettingsManager<T> : ISettingsManager<T>
    where T : class
{
    public static BaseSettingsManager<T>? Manager { get; private set; }

    public SettingsManager(JsonSerializerContext _ctx)
    {
        Manager = new(_ctx);
    }

    /// <inheritdoc />
    public void FirstRun(T data)
    {
        Manager!.FirstRun(data);
    }

    /// <inheritdoc />
    public T? Load()
    {
        return Manager!.Load()!;
    }

    /// <inheritdoc />
    public void Save(T data)
    {
        Manager!.Save(data);
    }
}
