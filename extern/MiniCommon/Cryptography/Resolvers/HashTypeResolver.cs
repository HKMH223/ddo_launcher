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
using MiniCommon.Cryptography.Enums;

namespace MiniCommon.Cryptography.Resolvers;

public static class HashTypeResolver
{
    /// <summary>
    /// Convert HashType to a hashtype string.
    /// </summary>
    public static string ToString(HashType? type) =>
        type switch
        {
            HashType.MD5 => "MD5",
            HashType.SHA1 => "SHA1",
            HashType.SHA256 => "SHA256",
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
}
