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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MiniCommon.Providers;

namespace MiniCommon.Decorators;

public static class TimingDecorator
{
    public static async Task<T> TimeAsync<T>(
        Func<Task<T>> func,
        [CallerMemberName] string methodName = ""
    )
    {
        Stopwatch sw = Stopwatch.StartNew();
        T? result = await func();
        sw.Stop();
        LogProvider.Benchmark("timing.output", methodName, sw.Elapsed.ToString("c"));
        return result;
    }

    public static async Task TimeAsync(Func<Task> func, [CallerMemberName] string memberName = "")
    {
        Stopwatch sw = Stopwatch.StartNew();
        await func();
        sw.Stop();
        LogProvider.Benchmark("timing.output", memberName, sw.Elapsed.ToString("c"));
    }
}
