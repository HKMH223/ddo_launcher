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

using MiniCommon.Web.Models;

namespace MiniCommon.Web.Interfaces;

public interface ITcp
{
    /// <summary>
    /// Ensure and validate the tcp clients connection.
    /// </summary>
    public static abstract bool EnsureConnection(string addr, string port);

    /// <summary>
    /// Construct an http request into a string.
    /// </summary>
    public static abstract string ConstructHttpRequest(HttpRequest httpRequest);

    /// <summary>
    /// Create a request as the tcp client.
    /// </summary>
    public static abstract string Request(string request);
}
