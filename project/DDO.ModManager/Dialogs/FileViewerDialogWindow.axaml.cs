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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using DDO.ModManager.Base;
using DDO.ModManager.Base.Models;
using DDO.ModManager.Base.NativePC.Providers;
using MiniCommon.Extensions;
using MiniCommon.IO;
using MiniCommon.IO.Helpers;
using MiniCommon.Logger.Enums;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using MiniCommon.Validation.Validators;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace DDO.ModManager.Dialogs;

#pragma warning disable RCS1043
public partial class FileViewerDialogWindow : Window, INotifyPropertyChanged
{
    private readonly Settings _runtimeSettings;
    public new event PropertyChangedEventHandler? PropertyChanged;

    private List<string>? _files;
    private static string? _source;
    private static string? _destination;

    private string? _filterRegex;
    public string? FilterRegex
    {
        get => _filterRegex;
        set
        {
            if (_filterRegex != value)
            {
                _filterRegex = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isAllEnabled;
    public bool IsAllEnabled
    {
        get => _isAllEnabled;
        set
        {
            if (_isAllEnabled != value)
            {
                _isAllEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<File> _filesList = [];
    public ObservableCollection<File> FilesList
    {
        get => _filesList;
        set
        {
            if (_filesList != value)
            {
                _filesList = value;
                OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public FileViewerDialogWindow()
    {
        if (RuntimeManager.RuntimeSettings is null)
            LogProvider.Warn("log.unhandled.exception", "Settings is null");

        _runtimeSettings = RuntimeManager.RuntimeSettings ?? new Settings();
        _files = FileListProvider.GetFiles(_runtimeSettings);
        FilterRegex = ".*";

        InitializeComponent();
        DataContext = this;
    }

    private void Filter_Click(object? sender, RoutedEventArgs e)
    {
        if (Validate.For.IsNullOrWhiteSpace([FilterRegex]))
            return;
        if (_runtimeSettings.RecheckOnFilter == true)
            _files = FileListProvider.GetFiles(_runtimeSettings);
        if (FilterRegex?.StartsWith("run:") == true)
        {
            RunFileViewerData();
            return;
        }
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

    private static void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is File file)
        {
            string filePath = VFS.GetFullPath(file.FileName!)
                .NormalizePath()
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (!VFS.Exists(filePath))
                return;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ProcessHelper.RunProcess("explorer.exe", $"/select,\"{filePath}\"", "", false);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                ProcessHelper.RunProcess("open", $"-R \"{filePath}\"", "", false);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ProcessHelper.RunProcess("xdg-open", $"\"{VFS.GetDirectoryName(filePath)}\"", "", false);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
    }

    private void CheckAllBox_Checked(object sender, RoutedEventArgs e)
    {
        if (FilesList.Count == 0 || FilesList.Count > _runtimeSettings.MaxDisableAll)
            return;
        foreach (File file in FilesList)
            file.FileName = ToggleFileState(file, true);
    }

    private void CheckAllBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (FilesList.Count == 0 || FilesList.Count > _runtimeSettings.MaxDisableAll)
            return;
        foreach (File file in FilesList)
            file.FileName = ToggleFileState(file, false);
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.DataContext is File file)
            file.FileName = ToggleFileState(file, true);
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.DataContext is File file)
            file.FileName = ToggleFileState(file, false);
    }

    /// <summary>
    /// Run through a file viewer data file.
    /// </summary>
    public void RunFileViewerData()
    {
        string? filepath = FilterRegex?.Replace("run:", string.Empty);
        if (Validate.For.IsNullOrWhiteSpace([filepath]))
            return;
        if (!VFS.Exists(filepath!))
            return;
        FileViewerProvider.Process(_runtimeSettings, filepath!);
    }

    /// <summary>
    /// Toggle the file state and update the UI thread to reflect the changes.
    /// </summary>
    public string? ToggleFileState(File file, bool enable) =>
        FileListProvider.ToggleFileState(
            _runtimeSettings,
            file.FileName,
            (string? src, string? dst, bool? _) =>
            {
                LogProvider.Info("ntpc.move", src!, dst!);

#pragma warning disable S2696
                _source = src;
                _destination = dst;
#pragma warning restore S2696

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
            },
            enable
        );

    /// <summary>
    /// Task to move a file from source to destination.
    /// </summary>
    private static Task MoveFileTask()
    {
        if (Validate.For.IsNullOrWhiteSpace([_source, _destination], NativeLogLevel.Fatal))
            return Task.CompletedTask;
        VFS.MoveFile(_source!, _destination!);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Task to build a list of files from regex and search settings.
    /// </summary>
    private DispatcherOperation BuildListTask()
    {
        List<File>? result = FileListProvider
            .FileList(_runtimeSettings, _files ?? Validate.For.EmptyList<string>(), FilterRegex!)
            .ConvertAll(f => new File { FileName = f.FileName, IsEnabled = f.IsEnabled ?? true });

        return Dispatcher.UIThread.InvokeAsync(() =>
        {
            FilesList.Clear();
            foreach (File? file in result)
                FilesList.Add(file);
        });
    }
}
