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
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using DDO.Launcher.Base.Helpers;
using DDO.Launcher.Base.Managers;
using DDO.Launcher.Base.Models;
using DDO.Launcher.Base.Providers;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using MiniCommon.Validation.Validators;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace DDO.Launcher.Dialogs;

public partial class SettingsDialogWindow : Window, INotifyPropertyChanged
{
    private string _executable = string.Empty;
    private string _serverName = string.Empty;
    private string _accountAPI = string.Empty;
    private string _downloadIP = string.Empty;
    private string _downloadPort = string.Empty;
    private string _lobbyIP = string.Empty;
    private string _lobbyPort = string.Empty;
    private bool _requireAdmin = false;
    private bool _localMode = false;

    private ObservableCollection<ServerInfo>? _serverInfoList;
    private ServerInfo? _selectedServerInfo;

    private readonly Settings _settings;

    public new event PropertyChangedEventHandler? PropertyChanged;

    public string Executable
    {
        get => _executable;
        set
        {
            if (_executable != value)
            {
                _executable = value;
                _settings.Executable = _executable;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Executable)));
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
                _settings.ServerInfo!.ServerName = _serverName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServerName)));
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
                _settings.ServerInfo!.AccountAPI = _accountAPI;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccountAPI)));
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
                _settings.ServerInfo!.DownloadIP = _downloadIP;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadIP)));
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
                _settings.ServerInfo!.DownloadPort = _downloadPort;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadPort)));
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
                _settings.ServerInfo!.LobbyIP = _lobbyIP;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LobbyIP)));
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
                _settings.ServerInfo!.LobbyPort = _lobbyPort;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LobbyPort)));
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
                _settings.RequireAdmin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RequireAdmin)));
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
                _settings.LocalMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalMode)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServerInfoList)));
            }
        }
    }

    public ServerInfo SelectedServerInfo
    {
        get => _selectedServerInfo ?? _settings.ServerInfos![0];
        set
        {
            if (_selectedServerInfo != value)
            {
                _selectedServerInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedServerInfo)));
            }

            if (value != null)
            {
                _settings.ServerInfo = value;
                UpdateComponents();
            }
        }
    }

    public SettingsDialogWindow()
    {
        if (ServiceManager.Settings is null)
            NotificationProvider.Warn("log.unhandled.exception", "Settings is null");

        _settings = ServiceManager.Settings ?? new Settings();
        _executable = _settings.Executable ?? Validate.For.EmptyString();
        _serverName = _settings.ServerInfo!.ServerName ?? Validate.For.EmptyString();
        _accountAPI = _settings.ServerInfo!.AccountAPI ?? Validate.For.EmptyString();
        _downloadIP = _settings.ServerInfo!.DownloadIP ?? Validate.For.EmptyString();
        _downloadPort = _settings.ServerInfo!.DownloadPort ?? Validate.For.EmptyString();
        _lobbyIP = _settings.ServerInfo!.LobbyIP ?? Validate.For.EmptyString();
        _lobbyPort = _settings.ServerInfo!.LobbyPort ?? Validate.For.EmptyString();
        _requireAdmin = _settings.RequireAdmin ?? false;
        _localMode = _settings.LocalMode ?? false;

        InitializeComponent();
        _serverInfoList = [.. _settings.ServerInfos!];
        ServerInfoList.CollectionChanged += (sender, args) => _settings.ServerInfos = [.. ServerInfoList];
        DataContext = this;
    }

    private void UpdateComponents()
    {
        ServerName = _settings.ServerInfo!.ServerName ?? Validate.For.EmptyString();
        AccountAPI = _settings.ServerInfo!.AccountAPI ?? Validate.For.EmptyString();
        DownloadIP = _settings.ServerInfo!.DownloadIP ?? Validate.For.EmptyString();
        DownloadPort = _settings.ServerInfo!.DownloadPort ?? Validate.For.EmptyString();
        LobbyIP = _settings.ServerInfo!.LobbyIP ?? Validate.For.EmptyString();
        LobbyPort = _settings.ServerInfo!.LobbyPort ?? Validate.For.EmptyString();
    }

    private void HashWrite_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        Task.Run(DDOVerifier.Write)
            .ContinueWith(_ =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    IsHitTestVisible = true;
                    Topmost = true;
                });
            });
    }

    private void HashVerify_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        Task.Run(DDOVerifier.Verify)
            .ContinueWith(_ =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    IsHitTestVisible = true;
                    Topmost = true;
                });
            });
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        Task.Run(SaveTask)
            .ContinueWith(_ =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    IsHitTestVisible = true;
                    Topmost = true;
                });
            });
    }

    private void SaveTask() => SettingsProvider.Save(_settings);

    private void AddToServerList_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        Task.Run(AddToServerListTask)
            .ContinueWith(_ =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    IsHitTestVisible = true;
                    Topmost = true;
                });
            });
    }

    private void AddToServerListTask()
    {
        if (Validate.For.IsNull(_settings.ServerInfos))
            return;
        if (
            Validate.For.IsNullOrWhiteSpace(
                [_serverName, _accountAPI, _downloadIP, _downloadPort, _lobbyIP, _lobbyPort]
            )
        )
        {
            return;
        }
        _settings.ServerInfos!.Add(
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
        ServerInfoList = [.. _settings.ServerInfos];
        SettingsProvider.Save(_settings);
    }

    private void RemoveFromServerList_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        Task.Run(RemoveFromServerListTask)
            .ContinueWith(_ =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    IsHitTestVisible = true;
                    Topmost = true;
                });
            });
    }

    private void RemoveFromServerListTask()
    {
        if (Validate.For.IsNull(_settings.ServerInfos))
            return;
        if (Validate.For.IsNullOrWhiteSpace([_serverName]))
            return;
        _settings.ServerInfos!.RemoveAll(a => a.ServerName == _serverName);
        ServerInfoList = [.. _settings.ServerInfos];
        SettingsProvider.Save(_settings);
    }
}
