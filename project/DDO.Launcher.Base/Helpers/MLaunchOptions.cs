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
using MiniCommon.Models;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using MiniCommon.Validation.Validators;

namespace DDO.Launcher.Base.Helpers;

public static class MLaunchOptions
{
    /// <summary>
    /// The default arguments to launch the process with.
    /// </summary>
    public static MOption[]? DefaultArguments(Settings settings, string token)
    {
        if (Validate.For.IsNull(settings))
            return null;

        return
        [
            new MOption { Arg = "addr={0}", ArgParams = [settings.LobbyIP ?? Validate.For.EmptyString()] },
            new MOption { Arg = "port={0}", ArgParams = [settings.LobbyPort ?? Validate.For.EmptyString()] },
            new MOption { Arg = "token={0}", ArgParams = [token ?? Validate.For.EmptyString()] },
            new MOption
            {
                Arg = "DL=http://{0}:{1}/win/",
                ArgParams =
                [
                    settings.DownloadIP ?? Validate.For.EmptyString(),
                    settings.DownloadPort ?? Validate.For.EmptyString(),
                ],
            },
            new MOption { Arg = "LVer={0}", ArgParams = ["03.04.003.20181115.0"] },
            new MOption { Arg = "RVer={0}", ArgParams = ["3040008"] },
        ];
    }
}
