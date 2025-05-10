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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Threading;
using DDO.Launcher.Base;
using DDO.Launcher.Base.Helpers;
using DDO.Launcher.Base.Models;
using MiniCommon.IO;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using MiniCommon.Validation.Validators;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace DDO.Launcher.Dialogs;

#pragma warning disable RCS1043
public partial class SettingsDialogWindow : Window, INotifyPropertyChanged
{
    private string _executable = string.Empty;
    private string _serverName = string.Empty;
    private string _accountAPI = string.Empty;
    private string _downloadIP = string.Empty;
    private string _downloadPort = string.Empty;
    private string _lobbyIP = string.Empty;
    private string _lobbyPort = string.Empty;
    private bool _requireAdmin;
    private bool _localMode;

    private ObservableCollection<ServerInfo>? _serverInfoList;
    private ServerInfo? _selectedServerInfo;

    private readonly Settings _runtimeSettings;

    public new event PropertyChangedEventHandler? PropertyChanged;

    public string Executable
    {
        get => _executable;
        set
        {
            if (_executable != value)
            {
                _executable = value;
                _runtimeSettings.Executable = _executable;
                OnPropertyChanged();
            }
        }
    }

    public string ServerName
    {
        get => _serverName;
        set
        {
            if (_serverName != value)
            {
                _serverName = value;
                _runtimeSettings.ServerInfo!.ServerName = _serverName;
                OnPropertyChanged();
            }
        }
    }

    public string AccountAPI
    {
        get => _accountAPI;
        set
        {
            if (_accountAPI != value)
            {
                _accountAPI = value;
                _runtimeSettings.ServerInfo!.AccountAPI = _accountAPI;
                OnPropertyChanged();
            }
        }
    }

    public string DownloadIP
    {
        get => _downloadIP;
        set
        {
            if (_downloadIP != value)
            {
                _downloadIP = value;
                _runtimeSettings.ServerInfo!.DownloadIP = _downloadIP;
                OnPropertyChanged();
            }
        }
    }

    public string DownloadPort
    {
        get => _downloadPort;
        set
        {
            if (_downloadPort != value)
            {
                _downloadPort = value;
                _runtimeSettings.ServerInfo!.DownloadPort = _downloadPort;
                OnPropertyChanged();
            }
        }
    }

    public string LobbyIP
    {
        get => _lobbyIP;
        set
        {
            if (_lobbyIP != value)
            {
                _lobbyIP = value;
                _runtimeSettings.ServerInfo!.LobbyIP = _lobbyIP;
                OnPropertyChanged();
            }
        }
    }

    public string LobbyPort
    {
        get => _lobbyPort;
        set
        {
            if (_lobbyPort != value)
            {
                _lobbyPort = value;
                _runtimeSettings.ServerInfo!.LobbyPort = _lobbyPort;
                OnPropertyChanged();
            }
        }
    }

    public bool RequireAdmin
    {
        get => _requireAdmin;
        set
        {
            if (_requireAdmin != value)
            {
                _requireAdmin = value;
                _runtimeSettings.RequireAdmin = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LocalMode
    {
        get => _localMode;
        set
        {
            if (_localMode != value)
            {
                _localMode = value;
                _runtimeSettings.LocalMode = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<ServerInfo> ServerInfoList
    {
        get => _serverInfoList!;
        set
        {
            if (_serverInfoList != value)
            {
                _serverInfoList = value;
                OnPropertyChanged();
            }
        }
    }

    public ServerInfo SelectedServerInfo
    {
        get => _selectedServerInfo ?? _runtimeSettings.ServerInfos![0];
        set
        {
            if (_selectedServerInfo != value)
            {
                _selectedServerInfo = value;
                OnPropertyChanged();
            }

            if (value != null)
            {
                _runtimeSettings.ServerInfo = value;
                UpdateComponents();
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public SettingsDialogWindow()
    {
        if (RuntimeManager.RuntimeSettings is null)
            LogProvider.Warn("log.unhandled.exception", "Settings is null");

        _runtimeSettings = RuntimeManager.RuntimeSettings ?? new Settings();
        _executable = _runtimeSettings.Executable ?? Validate.For.EmptyString();
        _serverName = _runtimeSettings.ServerInfo!.ServerName ?? Validate.For.EmptyString();
        _accountAPI = _runtimeSettings.ServerInfo!.AccountAPI ?? Validate.For.EmptyString();
        _downloadIP = _runtimeSettings.ServerInfo!.DownloadIP ?? Validate.For.EmptyString();
        _downloadPort = _runtimeSettings.ServerInfo!.DownloadPort ?? Validate.For.EmptyString();
        _lobbyIP = _runtimeSettings.ServerInfo!.LobbyIP ?? Validate.For.EmptyString();
        _lobbyPort = _runtimeSettings.ServerInfo!.LobbyPort ?? Validate.For.EmptyString();
        _requireAdmin = _runtimeSettings.RequireAdmin ?? false;
        _localMode = _runtimeSettings.LocalMode ?? false;

        InitializeComponent();
        _serverInfoList = [.. _runtimeSettings.ServerInfos!];
        ServerInfoList.CollectionChanged += (sender, args) => _runtimeSettings.ServerInfos = [.. ServerInfoList];
        DataContext = this;
    }

    private void UpdateComponents()
    {
        ServerName = _runtimeSettings.ServerInfo!.ServerName ?? Validate.For.EmptyString();
        AccountAPI = _runtimeSettings.ServerInfo!.AccountAPI ?? Validate.For.EmptyString();
        DownloadIP = _runtimeSettings.ServerInfo!.DownloadIP ?? Validate.For.EmptyString();
        DownloadPort = _runtimeSettings.ServerInfo!.DownloadPort ?? Validate.For.EmptyString();
        LobbyIP = _runtimeSettings.ServerInfo!.LobbyIP ?? Validate.For.EmptyString();
        LobbyPort = _runtimeSettings.ServerInfo!.LobbyPort ?? Validate.For.EmptyString();
    }

    private void HashWrite_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        TaskManager
            .Run(DDOVerifier.Write)
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

    private void HashVerify_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        TaskManager
            .Run(DDOVerifier.Verify)
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

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        TaskManager
            .Run(SaveTask)
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

    private void SaveTask() => RuntimeManager.SettingsManager?.Save(_runtimeSettings);

    private void AddToServerList_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        TaskManager
            .Run(AddToServerListTask)
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

    private void AddToServerListTask()
    {
        if (Validate.For.IsNull(_runtimeSettings.ServerInfos))
            return;
        if (
            Validate.For.IsNullOrWhiteSpace(
                [_serverName, _accountAPI, _downloadIP, _downloadPort, _lobbyIP, _lobbyPort]
            )
        )
        {
            return;
        }
        _runtimeSettings.ServerInfos!.Add(
            new()
            {
                ServerName = _serverName,
                AccountAPI = _accountAPI,
                DownloadIP = _downloadIP,
                DownloadPort = _downloadPort,
                LobbyIP = _lobbyIP,
                LobbyPort = _lobbyPort,
            }
        );
        ServerInfoList = [.. _runtimeSettings.ServerInfos];
        RuntimeManager.SettingsManager?.Save(_runtimeSettings);
    }

    private void RemoveFromServerList_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        TaskManager
            .Run(RemoveFromServerListTask)
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

    private void RemoveFromServerListTask()
    {
        if (Validate.For.IsNull(_runtimeSettings.ServerInfos))
            return;
        if (Validate.For.IsNullOrWhiteSpace([_serverName]))
            return;
        _runtimeSettings.ServerInfos!.RemoveAll(a => a.ServerName == _serverName);
        ServerInfoList = [.. _runtimeSettings.ServerInfos];
        RuntimeManager.SettingsManager?.Save(_runtimeSettings);
    }
}
