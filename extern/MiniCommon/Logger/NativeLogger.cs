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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MiniCommon.IO;
using MiniCommon.Logger.Enums;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;

namespace MiniCommon.Logger;

public partial class NativeLogger : ILogger
{
    [LibraryImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    [LibraryImport("kernel32", SetLastError = true)]
    private static partial IntPtr GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [LibraryImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    private const int STD_INPUT_HANDLE = -10;
    private const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
    private const uint ENABLE_EXTENDED_FLAGS = 0x0080;

    private readonly NativeLogLevel _minLevel = NativeLogLevel.Debug;
    private readonly CensorLevel _censorLevel = CensorLevel.NONE;

    /// <summary>
    /// Create a new NativeLogger and allocate a console for it.
    /// </summary>
    public NativeLogger()
    {
        _ = AllocConsole();
        DisableQuickEditMode();
        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
    }

    /// <summary>
    /// Create a new NativeLogger with a minimum log level and allocate a console for it.
    /// </summary>
    public NativeLogger(NativeLogLevel minLevel, CensorLevel censorLevel = CensorLevel.NONE)
    {
        _ = AllocConsole();
        _minLevel = minLevel;
        _censorLevel = censorLevel;
        DisableQuickEditMode();
        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
    }

    /// <summary>
    /// Quick edit disables console i/o when text is highlighted.
    /// This causes a disruption in the application. We want it disabled.
    /// </summary>
    private static void DisableQuickEditMode()
    {
        IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);
        if (GetConsoleMode(handle, out uint mode))
        {
            mode &= ~ENABLE_QUICK_EDIT_MODE;
            mode |= ENABLE_EXTENDED_FLAGS;
            SetConsoleMode(handle, mode);
        }
    }

    public Task Base(NativeLogLevel level, string message) => WriteToStdout(level, message);

    public Task Base(NativeLogLevel level, string format, params object[] args) =>
        WriteToStdout(level, string.Format(format, args));

    public Task Benchmark(string message) => WriteToStdout(NativeLogLevel.Benchmark, message);

    public Task Benchmark(string format, params object[] args) =>
        WriteToStdout(NativeLogLevel.Benchmark, string.Format(format, args));

    public Task Debug(string message) => WriteToStdout(NativeLogLevel.Debug, message);

    public Task Debug(string format, params object[] args) =>
        WriteToStdout(NativeLogLevel.Debug, string.Format(format, args));

    public Task Info(string message) => WriteToStdout(NativeLogLevel.Info, message);

    public Task Info(string format, params object[] args) =>
        WriteToStdout(NativeLogLevel.Info, string.Format(format, args));

    public Task Warn(string message) => WriteToStdout(NativeLogLevel.Warn, message);

    public Task Warn(string format, params object[] args) =>
        WriteToStdout(NativeLogLevel.Warn, string.Format(format, args));

    public Task Error(string message) => WriteToStdout(NativeLogLevel.Error, message);

    public Task Error(string format, params object[] args) =>
        WriteToStdout(NativeLogLevel.Error, string.Format(format, args));

    public Task Fatal(string message) => WriteToStdout(NativeLogLevel.Fatal, message);

    public Task Fatal(string format, params object[] args) =>
        WriteToStdout(NativeLogLevel.Fatal, string.Format(format, args));

    public Task Native(string message) => WriteToStdout(NativeLogLevel.Error, message);

    public Task Native(string format, params object[] args) =>
        WriteToStdout(NativeLogLevel.Native, string.Format(format, args));

    /// <summary>
    /// Handle incoming exception objects.
    /// </summary>
    private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            NotificationProvider.Error("log.unhandled.exception", ex.ToString());
        }
        else
        {
            NotificationProvider.Error(
                "log.unhandled.object",
                e.ExceptionObject.ToString() ?? Validate.For.EmptyString()
            );
        }
    }

    /// <summary>
    /// Write a formatted message to stdout.
    /// </summary>
    private Task<bool> WriteToStdout(NativeLogLevel level, string message)
    {
        if ((int)level < (int)_minLevel)
            return Task.FromResult(true);

        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.Write($"[{DateTime.Now.ToLongTimeString()}]");
        Console.ForegroundColor = (ConsoleColor)level;
        Console.Write($"[{level.ToString().ToUpper()}] ");
        Console.ForegroundColor = ConsoleColor.Gray;

        switch (_censorLevel)
        {
            case CensorLevel.REDACT:
                Console.WriteLine(VFS.GetRedactedPath(message));
                break;
            default:
                Console.WriteLine(message);
                break;
        }

        return Task.FromResult(true);
    }
}
