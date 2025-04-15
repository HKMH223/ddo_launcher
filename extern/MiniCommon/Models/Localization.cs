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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiniCommon.Models;

public class Localization
{
    public Dictionary<string, string> Entries { get; set; }

    public Localization()
    {
        Entries ??= [];
        Entries.Add("localization.service", "localization.service");
        Entries.Add("log", "{0}");
    }

    public static Dictionary<string, string> Default()
    {
        Dictionary<string, string> _entries = [];

        #region Base Logging Functionality
        _entries.Add("localization.service", "localization.service");
        _entries.Add("log", "{0}");
        _entries.Add("log.stack.trace", "{0}\n{1}");
        _entries.Add("log.initialized", "Intialized Logger");
        _entries.Add("log.unhandled.exception", "Unhandled exception: {0}");
        _entries.Add("log.unhandled.object", "Unhandled non-exception object: {0}");
        #endregion // Base Logging Functionality

        #region Benchmarking and Stopwatch
        _entries.Add("timing.output", "Method: {0}() took {1} to complete.");
        #endregion // Benchmarking and Stopwatch

        #region File Patching
        _entries.Add("patch.success", "({0}/{1}) Patch applied: {2} -> {3} at 0x{4}");
        #endregion // File Patching

        #region Tcp Requests
        _entries.Add("tcp.connect", "TCP Connection: {0}");
        _entries.Add("tcp.request", "TCP Request: {0}");
        _entries.Add("tcp.error.response", "TCP Response Error: {0}");
        #endregion // Tcp Requests

        #region Http Requests
        _entries.Add("request.get.start", "Sending GET request to: {0}");
        _entries.Add("request.get.exists", "File already exists: {0}");
        _entries.Add("request.get.success", "Successfully received response from: {0} in {1}.");
        #endregion // Http Requests

        #region Hash Verifier
        _entries.Add("hash.success", "File: {0} ({1}) matches target hash: {2}");
        _entries.Add("hash.writing", "Adding file: {0} ({1})");
        #endregion // Hash Verifier

        #region Error Handling
        _entries.Add("error.hash.nohash", "No hash file exists in '{0}'");
        _entries.Add("error.hash.missing", "File: {0} ({1}) is missing.");
        _entries.Add("error.hash.invalid", "File: {0} ({1}) does not match target hash: {2}");
        _entries.Add("error.patch", "({0}/{1}) Pattern not found: {2}");
        _entries.Add("error.download", "An error occurred attempting to request: '{0}'");
        _entries.Add("error.request", "An error occurred attempting to request: '{0}'\n{1}\n{2}");
        _entries.Add("error.readfile", "An error occurred attempting to load: '{0}'");
        _entries.Add("error.writefile", "An error occurred attempting to write: '{0}'");
        _entries.Add("error.parse", "An error occurred attempting to parse: '{0}' as '{1}'");
        _entries.Add(
            "error.validation.object",
            "{0}, {1} cannot be null. Method: {2}() in {3}:{4}"
        );
        _entries.Add(
            "error.validation.list",
            "{0}, {1} cannot be null, or empty. Method: {2}() in {3}:{4}"
        );
        _entries.Add(
            "error.validation.string",
            "{0} cannot be null, empty, or whitespace. Method: {1}() in {2}:{3}"
        );
        _entries.Add("error.validation.list-shim", "Empty list. {0}() in {1}:{2}");
        _entries.Add("error.validation.string-shim", "Empty string. {0}() in {1}:{2}");
        _entries.Add("error.validation.class-shim", "Empty class. {0}() in {1}:{2}");
        _entries.Add("error.admin.required", "Administrator permissions are required.");
        _entries.Add(
            "error.badpath",
            "Path '{0}' contains problematic or reserved location '{1}'."
        );
        _entries.Add("stack.trace.null", "No stack trace provided.");
        #endregion // Error Handling

        #region Command Line
        _entries.Add("commandline.error", "Invalid command. Expects: {0}");
        _entries.Add("commandline.exit", "Press '{0}' to safely exit.");
        #endregion // Command Line

        #region Launcher
        _entries.Add("launcher.settings.save", "Saving launcher config: '{0}'.");
        _entries.Add("launcher.settings.load", "Loading launcher config: '{0}'.");
        _entries.Add("launcher.settings.setup", "Creating launcher config: '{0}'.");
        _entries.Add("launcher.settings.using", "Using launcher config at: '{0}'.");
        _entries.Add(
            "launcher.settings.missing",
            "'{0}' could not be read, make sure it is located in '{1}' and try again."
        );
        #endregion // Launcher

        #region Avalonia
        _entries.Add("avalonia.open.dialog", "Opening dialog box: {0}.");
        _entries.Add("avalonia.close.dialog", "Closing dialog box: {0}.");
        _entries.Add("avalonia.title.error", "Error");
        _entries.Add("avalonia.title.confirm", "Confirm");
        _entries.Add("avalonia.title.modding", "Modding");
        _entries.Add("avalonia.title.settings", "Settings");
        _entries.Add("avalonia.irreversable.message", "This action is irreversible, are you sure?");
        #endregion // Avalonia

        #region Code Analyzers
        _entries.Add(
            "analyzer.error.namespace",
            "'{0}' does not contain a valid namespace: '{1}' expects '{2}'"
        );
        _entries.Add("analyzer.error.license", "'{0}' does not contain a valid license.");
        _entries.Add(
            "analyzer.error.localization",
            "'{0}', '{1}' is not a valid localization key."
        );
        _entries.Add("analyzer.output", "[{0}] Done: '{1}', Fail: '{2}', Total: '{3}'");
        _entries.Add("analyzer.line.output", "[{0}] Total: '{1}', Average (Per File): '{2}'.");
        #endregion // Code Analyzers

        return _entries;
    }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
)]
[JsonSerializable(typeof(Localization))]
internal partial class LocalizationContext : JsonSerializerContext;
