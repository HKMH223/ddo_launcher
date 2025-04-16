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
using DDO.ModManager.Base.Enums;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;

namespace DDO.ModManager.Base.Resolvers;

public static class KnownGameResolver
{
    /// <summary>
    /// Convert KnownGame to a name string.
    /// </summary>
    public static string ToString(KnownGame? type) =>
        type switch
        {
            KnownGame.DDO => "ddo" ?? Validate.For.EmptyString(),
            KnownGame.MHW => "mhw" ?? Validate.For.EmptyString(),
            KnownGame.MHRISE => "mhrise" ?? Validate.For.EmptyString(),
            KnownGame.MHWILDS => "mhwilds" ?? Validate.For.EmptyString(),
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
}
