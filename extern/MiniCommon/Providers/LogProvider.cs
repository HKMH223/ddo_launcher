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
using MiniCommon.Logger.Enums;
using MiniCommon.Models;

namespace MiniCommon.Providers;

public static class LogProvider
{
    private static readonly List<LogMessage> _messages = [];
    public static int MaxSize { get; set; } = 256;
    public static bool CacheMessages { get; set; }

    public static void Add(LogMessage item) => _messages.Add(item);

    public static void BenchmarkLog(params string[] _params) =>
        LogInternal(NativeLogLevel.Benchmark, "log", _params);

    public static void Benchmark(string id, params string[] _params) =>
        LogInternal(NativeLogLevel.Benchmark, id, _params);

    public static void DebugLog(params string[] _params) =>
        LogInternal(NativeLogLevel.Debug, "log", _params);

    public static void Debug(string id, params string[] _params) =>
        LogInternal(NativeLogLevel.Debug, id, _params);

    public static void WarnLog(params string[] _params) =>
        LogInternal(NativeLogLevel.Warn, "log", _params);

    public static void Warn(string id, params string[] _params) =>
        LogInternal(NativeLogLevel.Warn, id, _params);

    public static void ErrorLog(params string[] _params) =>
        LogInternal(NativeLogLevel.Error, "log", _params);

    public static void Error(string id, params string[] _params) =>
        LogInternal(NativeLogLevel.Error, id, _params);

    public static void FatalLog(params string[] _params) =>
        LogInternal(NativeLogLevel.Fatal, "log", _params);

    public static void Fatal(string id, params string[] _params) =>
        LogInternal(NativeLogLevel.Fatal, id, _params);

    public static void InfoLog(params string[] _params) =>
        LogInternal(NativeLogLevel.Info, "log", _params);

    public static void Info(string id, params string[] _params) =>
        LogInternal(NativeLogLevel.Info, id, _params);

    public static void NativeLog(params string[] _params) =>
        LogInternal(NativeLogLevel.Native, "log", _params);

    public static void Native(string id, params string[] _params) =>
        LogInternal(NativeLogLevel.Native, id, _params);

    public static void PrintLog(NativeLogLevel level, params string[] _params) =>
        LogInternal(level, "log", _params);

    public static void Log(NativeLogLevel level, string id) => LogInternal(level, id);

    public static void Log(NativeLogLevel level, string id, params string[] _params) =>
        LogInternal(level, id, _params);

    public static void Clear() => _messages.Clear();

    /// <summary>
    /// Execute a user-defined method when a LogMessage is added to the list.
    /// </summary>
    public static void OnLogMessageAdded(Action<LogMessage> func)
    {
        LogMessage.OnLogMessageAdded += func;
        LogMessage.OnLogMessageAdded += Manage;
    }

    /// <summary>
    /// Keep the LogMessage list in a rotating list of MaxSize.
    /// </summary>
    private static void Manage(LogMessage _)
    {
        if (_messages.Count > MaxSize)
            _messages.RemoveAt(0);
    }

    /// <summary>
    /// Base logging method.
    /// </summary>
    private static void LogInternal(NativeLogLevel level, string id, params string[] _params)
    {
        LogMessage msg = new(level, id, _params);
        if (CacheMessages)
            _messages.Add(msg);
    }

    /// <summary>
    /// Base logging method.
    /// </summary>
    private static void LogInternal(NativeLogLevel level, string id)
    {
        LogMessage msg = new(level, id);
        if (CacheMessages)
            _messages.Add(msg);
    }
}
