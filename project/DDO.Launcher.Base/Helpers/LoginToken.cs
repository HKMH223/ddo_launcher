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
using System.Security.Cryptography;
using System.Text;

namespace DDO.Launcher.Base.Helpers;

public static class LoginToken
{
    private const string TokenPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int LoginTokenLength = 20;
    public static readonly byte[] Salt = [0x1B, 0xC2, 0x3A, 0x17, 0x6F, 0x41, 0x84, 0x26, 0x48, 0x5E];

    public static string Generate(string key1, string key2)
    {
        string combinedKeys = key1 + key2 + Encoding.UTF8.GetString(Salt);
        byte[] combinedBytes = Encoding.UTF8.GetBytes(combinedKeys);
        byte[] hashBytes = SHA256.HashData(combinedBytes);
        long seed = BitConverter.ToInt64(hashBytes, 0);

        Random random = new(unchecked((int)seed));
        StringBuilder tokenBuilder = new();

        for (int i = 0; i < LoginTokenLength; i++)
        {
            int tokenPoolIndex = random.Next(0, TokenPool.Length);
            tokenBuilder.Append(TokenPool[tokenPoolIndex]);
        }

        return tokenBuilder.ToString();
    }
}
