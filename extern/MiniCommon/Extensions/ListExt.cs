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

using System.Collections.Generic;
using System.Linq;

namespace MiniCommon.Extensions;

public static class ListExt
{
    /// <summary>
    /// Moves a specified entry within a list to a new index. If the entry is not found, the list remains unchanged.
    /// </summary>
    public static List<T> MoveEntry<T>(this List<T> list, T entry, int newIndex)
    {
        int currentIndex = list.IndexOf(entry);

        if (currentIndex == -1)
            return list;

        list.RemoveAt(currentIndex);

        if (newIndex >= list.Count)
        {
            list.Add(entry);
        }
        else
        {
            list.Insert(newIndex, entry);
        }

        return list;
    }

    /// <summary>
    /// Merges two lists into one, removing duplicate entries. If either list is null, the other is returned.
    /// </summary>
    public static List<T> Merge<T>(this List<T>? first, List<T>? second)
    {
        if (first is null && second is null)
            return [];

        if (first is null)
            return new List<T>(second!);

        if (second is null)
            return new List<T>(first);

        return first.Concat(second).Distinct().ToList();
    }
}
