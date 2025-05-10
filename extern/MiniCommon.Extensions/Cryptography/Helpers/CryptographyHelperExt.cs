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
using System.IO.Hashing;
using System.Text;
using MiniCommon.IO;

namespace MiniCommon.Extensions.Cryptography.Helpers;

public static class CryptographyHelperExt
{
    /// <summary>
    /// Create a CRC32 hash from a filestream, and return as string.
    /// </summary>
    public static string CreateCRC32(string fileName, bool formatting)
    {
        if (!VFS.Exists(fileName))
            return string.Empty;

        byte[] hash = Crc32.Hash(VFS.ReadFile(fileName));

        if (formatting)
            return Convert.ToHexStringLower(hash).Replace("-", string.Empty);
        return BitConverter.ToString(hash);
    }

    /// <summary>
    /// Create a CRC32 hash from a string, and return as string.
    /// </summary>
    public static string CreateCRC32(string value, Encoding enc)
    {
        byte[] hash = Crc32.Hash(enc.GetBytes(value));
        StringBuilder stringBuilder = new();
        foreach (byte b in hash)
        {
            stringBuilder.Append(b.ToString("x2"));
        }
        return stringBuilder.ToString();
    }
}
