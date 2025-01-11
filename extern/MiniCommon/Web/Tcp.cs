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

using MiniCommon.Web.Abstractions;
using MiniCommon.Web.Interfaces;
using MiniCommon.Web.Models;

namespace MiniCommon.Web;

public class Tcp : ITcp
{
    public static ValidatedTcp TcpClient { get; } = new();

    /// <inheritdoc />
    public static bool EnsureConnection(string addr, string port)
    {
        return TcpClient.EnsureConnection(addr, port);
    }

    /// <inheritdoc />
    public static string ConstructHttpRequest(HttpRequest httpRequest)
    {
        return TcpClient.ConstructHttpRequest(httpRequest);
    }

    /// <inheritdoc />
    public static string Request(string request)
    {
        return TcpClient.Request(request);
    }
}
