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

using MiniCommon.Logger.Enums;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;
using MiniCommon.Web.Models;

namespace MiniCommon.Web.Abstractions;

public class ValidatedTcp : BaseTcp
{
    /// <inheritdoc />
    public override bool EnsureConnection(string addr, string port)
    {
        if (Validate.For.IsNullOrWhiteSpace([addr, port], NativeLogLevel.Fatal))
            return false;
        return base.EnsureConnection(addr, port);
    }

    /// <inheritdoc />
    public override string ConstructHttpRequest(HttpRequest httpRequest)
    {
        if (Validate.For.IsNull(httpRequest, NativeLogLevel.Fatal))
            return string.Empty;
        if (
            Validate.For.IsNullOrWhiteSpace(
                [
                    httpRequest.Method,
                    httpRequest.Version,
                    httpRequest.Path,
                    httpRequest.Address,
                    httpRequest.Port,
                    httpRequest.ContentType,
                    httpRequest.Content,
                ],
                NativeLogLevel.Fatal
            )
        )
        {
            return string.Empty;
        }
        return base.ConstructHttpRequest(httpRequest);
    }

    /// <inheritdoc />
    public override string Request(string request)
    {
        if (Validate.For.IsNullOrWhiteSpace([request], NativeLogLevel.Fatal))
            return string.Empty;
        return base.Request(request);
    }
}
