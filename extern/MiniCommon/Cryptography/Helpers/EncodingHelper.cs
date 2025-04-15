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
using System.Text;
using MiniCommon.Cryptography.Enums;

namespace MiniCommon.Cryptography.Helpers;

public static class EncodingHelper
{
    /// <summary>
    /// Decode a Base64 string, and return as string.
    /// </summary>
    public static string FromBase64(string value) => Decode(value, EncodeType.BASE64);

    /// <summary>
    /// Decode a Hex string, and return as string.
    /// </summary>
    public static string FromHex(string value) => Decode(value, EncodeType.HEX);

    /// <summary>
    /// Encode a string into Base64, and return as string.
    /// </summary>
    public static string ToBase64(string value) => Encode(value, EncodeType.BASE64);

    /// <summary>
    /// Encode a string into Hex, and return as string.
    /// </summary>
    public static string ToHex(string value) => Encode(value, EncodeType.HEX);

    /// <summary>
    /// Encode a string based on an EncodeType, and return as string.
    /// </summary>
    public static string Encode(string value, EncodeType encode) =>
        encode switch
        {
            EncodeType.BASE64 => Convert.ToBase64String(Encoding.Default.GetBytes(value)),
            EncodeType.HEX => Convert.ToHexString(Encoding.Default.GetBytes(value)),
            EncodeType.ASCII => Encoding.ASCII.GetString(Encoding.Default.GetBytes(value)),
            EncodeType.UTF8 => Encoding.UTF8.GetString(Encoding.Default.GetBytes(value)),
            _ => throw new ArgumentOutOfRangeException(nameof(encode), encode, null),
        };

    /// <summary>
    /// Decode a string based on an EncodeType, and return as string.
    /// </summary>
    public static string Decode(string value, EncodeType encode) =>
        encode switch
        {
            EncodeType.BASE64 => Encoding.UTF8.GetString(Convert.FromBase64String(value)),
            EncodeType.HEX => Encoding.UTF8.GetString(Convert.FromHexString(value)),
            EncodeType.ASCII => Encoding.ASCII.GetString(Encoding.Default.GetBytes(value)),
            EncodeType.UTF8 => Encoding.UTF8.GetString(Encoding.Default.GetBytes(value)),
            _ => throw new ArgumentOutOfRangeException(nameof(encode), encode, null),
        };
}
