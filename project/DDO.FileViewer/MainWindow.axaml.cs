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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using MiniCommon.BuildInfo;
using MiniCommon.Extensions;
using MiniCommon.Extensions.FileGlobber;
using MiniCommon.IO;
using MiniCommon.IO.Helpers;
using MiniCommon.IO.Models;
using MiniCommon.Logger.Enums;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace DDO.FileViewer;

#pragma warning disable RCS1043
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private string? _filterRegex;
    private ObservableCollection<File> _filesList = [];
    public new event PropertyChangedEventHandler? PropertyChanged;

    private string? _disabled;
    private static SimpleConfig? _config;
    private static string? _source;
    private static string? _destination;

    public string? FilterRegex
    {
        get => _filterRegex;
        set
        {
            if (_filterRegex != value)
            {
                _filterRegex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilterRegex)));
            }
        }
    }

    public ObservableCollection<File> FilesList
    {
        get => _filesList;
        set
        {
            if (_filesList != value)
            {
                _filesList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilesList)));
            }
        }
    }

    public sealed class File : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string? _fileName;
        public string? FileName
        {
            get => _fileName;
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileName)));
                }
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

#pragma warning disable S3010
        _config = SimpleConfigHelper.Read(
            new()
            {
                { "Directory", VFS.FileSystem.Cwd },
                { "Pattern", ".*" },
                { "Append", ".DISABLED" },
                { "MaxFiles", "255" },
            }
        );
#pragma warning restore S2696

        FilterRegex = ".*";
        if (Validate.For.IsNull(_config))
            return;
        if (Validate.For.IsNullOrWhiteSpace([FilterRegex]))
            return;
        IsHitTestVisible = false;
        Topmost = false;
        TaskManager
            .Run(BuildListTask)
            .ContinueWith(_ =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    IsHitTestVisible = true;
                    Topmost = true;
                    Topmost = false;
                });
            });
    }

    private static void Title_Initialized(object sender, EventArgs e)
    {
        if (sender is TextBlock { Text: not null } block)
            block.Text = AssemblyConstants.AssemblyName;
    }

    private static void Version_Initialized(object sender, EventArgs e)
    {
        if (sender is TextBlock { Text: not null } block)
            block.Text = $"v{ApplicationConstants.CurrentVersion}";
    }

    private void Filter_Click(object? sender, RoutedEventArgs e)
    {
        if (Validate.For.IsNull(_config))
            return;
        if (Validate.For.IsNullOrWhiteSpace([FilterRegex]))
            return;
        IsHitTestVisible = false;
        Topmost = false;
        TaskManager
            .Run(BuildListTask)
            .ContinueWith(_ =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    IsHitTestVisible = true;
                    Topmost = true;
                    Topmost = false;
                });
            });
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.DataContext is File file)
            file.FileName = ToggleFileState(file.FileName, true);
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.DataContext is File file)
            file.FileName = ToggleFileState(file.FileName, false);
    }

    /// <summary>
    /// Toggle the file's enabled state by appending or removing the disabled extension.
    /// </summary>
    private string? ToggleFileState(string? filepath, bool enable)
    {
        if (Validate.For.IsNullOrWhiteSpace([filepath]))
            return filepath;

        bool isDisabled = VFS.GetFileExtension(filepath!) == _disabled;

        if ((enable && isDisabled) || (!enable && !isDisabled))
        {
#pragma warning disable S2696
            _source = filepath;
            _destination = enable ? filepath![..filepath!.LastIndexOf(_disabled!)] : filepath + _disabled;
#pragma warning restore S2696

            LogProvider.Info("ntpc.move", _source!, _destination);

            IsHitTestVisible = false;
            Topmost = false;

            TaskManager
                .Run(MoveFileTask)
                .ContinueWith(_ =>
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        IsHitTestVisible = true;
                        Topmost = true;
                        Topmost = false;
                    });
                });

            return _destination;
        }

        return filepath;
    }

    private static Task MoveFileTask()
    {
        if (Validate.For.IsNullOrWhiteSpace([_source, _destination], NativeLogLevel.Fatal))
            return Task.CompletedTask;
        VFS.MoveFile(_source!, _destination!);
        return Task.CompletedTask;
    }

    private DispatcherOperation BuildListTask()
    {
        string[] files = VFS.GetFiles(
            _config!.GetValue("Directory"),
            _config!.GetValue("Pattern"),
            SearchOption.AllDirectories
        );
        _disabled = _config!.GetValue("Append");

        int maxFiles;
        string? maxFilesStr = _config!.GetValue("MaxFiles");
        if (!int.TryParse(maxFilesStr, out maxFiles) || maxFiles < 1)
            maxFiles = 255;

        Regex? regex = null;
        try
        {
            regex = new Regex(FilterRegex!, RegexOptions.IgnoreCase);
        }
        catch (Exception ex)
        {
            LogProvider.Error("log.unhandled.exception", ex.ToString());
        }

        if (Validate.For.IsNull(regex))
            return Dispatcher.UIThread.InvokeAsync(() => { });

        List<File>? result = files
            .Where(file => regex!.IsMatch(file))
            .Select(file => new File
            {
                FileName = file.NormalizePath(),
                IsEnabled = VFS.GetFileExtension(file) != _disabled,
            })
            .DistinctBy(f => f.FileName)
            .Take(maxFiles)
            .ToList();

        return Dispatcher.UIThread.InvokeAsync(() =>
        {
            FilesList.Clear();
            foreach (File? file in result)
                FilesList.Add(file);
        });
    }

    [GeneratedRegex(".*", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex DefaultFilterRegex();
}
