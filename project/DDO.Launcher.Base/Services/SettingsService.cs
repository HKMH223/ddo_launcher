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
using DDO.Launcher.Base.Models;
using DDO.Launcher.Base.Providers;
using MiniCommon.Logger;
using MiniCommon.Managers.Interfaces;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.Services;

public class SettingsService : IBaseService
{
    public static Settings? Settings { get; set; }

    public Task<bool> Initialize<T>(T? _)
    {
        try
        {
            SettingsProvider.FirstRun();
            Settings = SettingsProvider.Load();
            if (Validate.For.IsNull(Settings))
                return Task.FromResult(false);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex.ToString());
            return Task.FromResult(false);
        }
    }
}
