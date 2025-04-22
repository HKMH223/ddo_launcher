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
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Diff;

static class Program
{
    static string CalculateChecksum(string filePath)
    {
        using FileStream stream = File.OpenRead(filePath);
        using MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    static void CompareFolders(string folder1, string folder2)
    {
        Dictionary<string, string> fileChecksums1 = [];
        Dictionary<string, string> fileChecksums2 = [];

        try
        {
            foreach (string filePath in Directory.EnumerateFiles(folder1, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(folder1, filePath);
                fileChecksums1[relativePath] = CalculateChecksum(filePath);
            }

            foreach (string filePath in Directory.EnumerateFiles(folder2, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(folder2, filePath);
                fileChecksums2[relativePath] = CalculateChecksum(filePath);
            }

            foreach (KeyValuePair<string, string> entry in fileChecksums1)
            {
                if (!fileChecksums2.TryGetValue(entry.Key, out string? checksum))
                {
                    Console.WriteLine($"File {entry.Key} exists in folder1 but not in folder2");
                }
                else if (entry.Value != checksum)
                {
                    Console.WriteLine($"Checksums for file {entry.Key} do not match:");
                    Console.WriteLine($"  Folder1: {entry.Value}");
                    Console.WriteLine($"  Folder2: {checksum}");
                }
            }

            foreach (KeyValuePair<string, string> entry in fileChecksums2)
            {
                if (!fileChecksums1.ContainsKey(entry.Key))
                {
                    Console.WriteLine($"File {entry.Key} exists in folder2 but not in folder1");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: diff <folder1> <folder2>");
            Environment.Exit(1);
        }

        CompareFolders(args[0], args[1]);
    }
}
