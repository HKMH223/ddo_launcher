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
using System.IO;
using System.Net.Sockets;
using System.Text;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;
using MiniCommon.Web.Interfaces;
using MiniCommon.Web.Models;

namespace MiniCommon.Web.Abstractions;

public class BaseTcp : IBaseTcp
{
    private static TcpClient _tcpClient = new();

    public virtual TcpClient GetTcpClient() => _tcpClient;

    public virtual int ReceiveTimeout { get; set; } = 5000;

    public virtual int SendTimeout { get; set; } = 5000;

    public virtual Encoding Encoding { get; set; } = new UTF8Encoding(false);

    /// <inheritdoc />
    public virtual bool EnsureConnection(string addr, string port)
    {
        try
        {
            LogProvider.Info("tcp.connect", $"{addr}:{port}");

            if (_tcpClient is null)
            {
                _tcpClient = new() { ReceiveTimeout = ReceiveTimeout, SendTimeout = SendTimeout };
                _tcpClient.Connect(addr, int.Parse(port));

                return true;
            }

            if (!_tcpClient.Connected)
            {
                _tcpClient.ReceiveTimeout = ReceiveTimeout;
                _tcpClient.SendTimeout = SendTimeout;
                _tcpClient.Connect(addr, int.Parse(port));
            }

            return true;
        }
        catch (Exception ex)
        {
            LogProvider.Error(
                "log.stack.trace",
                ex.Message,
                ex.StackTrace ?? LocalizationProvider.Translate("stack.trace.null")
            );
        }

        return false;
    }

    /// <inheritdoc />
    public virtual string ConstructHttpRequest(HttpRequest httpRequest)
    {
        string request = $"{httpRequest.Method} {httpRequest.Path} HTTP/{httpRequest.Version}\r\n";
        request += $"Host: {httpRequest.Address}:{httpRequest.Port}\r\n";
        request += $"Content-Type: {httpRequest.ContentType}\r\n";
        request += $"Content-Length: {httpRequest.Content!.Length}\r\n";
        request += "Connection: close\r\n";
        request += "\r\n";
        request += httpRequest.Content;

        return request;
    }

    /// <inheritdoc />
    public virtual string Request(string request)
    {
        try
        {
            using NetworkStream stream = _tcpClient!.GetStream();
            using StreamWriter writer = new(stream, Encoding);
            using StreamReader reader = new(stream, Encoding);

            LogProvider.Info("tcp.request", request);
            writer.Write(request);
            writer.Flush();

            StringBuilder sb = new();
            string line;
            stream.ReadTimeout = 5000;

            while ((line = reader.ReadLine()!) is not null)
                sb.AppendLine(line);

            string response = sb.ToString();
            int bodyStartIndex = response.IndexOf("\r\n\r\n") + 4;
            string? responseBody = response[bodyStartIndex..];

            if (Validate.For.IsNullOrWhiteSpace([responseBody]))
                return string.Empty;

            return responseBody;
        }
        catch (Exception ex)
        {
            LogProvider.Error(
                "log.stack.trace",
                ex.Message,
                ex.StackTrace ?? LocalizationProvider.Translate("stack.trace.null")
            );
        }

        return string.Empty;
    }
}
