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

using DDO.ModManager.Base.NativePC.Models;
using MiniCommon.Extensions;

namespace DDO.ModManager.Base.NativePC.Extensions;

public static class NtPcRulesExt
{
    /// <summary>
    /// Concat one NtPcRules object with another.
    /// </summary>
    public static void ConcatWith(this NtPcRules target, NtPcRules source)
    {
        target.Addons = target.Addons.Merge(source.Addons);
        target.Exclusions = target.Exclusions.Merge(source.Exclusions);
        target.LoadOrders = target.LoadOrders.Merge(source.LoadOrders);
        target.Renames = target.Renames.Merge(source.Renames);
        target.IgnorePrefixes = target.IgnorePrefixes.Merge(source.IgnorePrefixes);
    }
}
