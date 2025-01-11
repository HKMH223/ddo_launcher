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

using System.ComponentModel;
using Avalonia.Controls;
using DDO.Launcher.Base.Managers;
using DDO.Launcher.Base.Models;
using DDO.Launcher.Base.Providers;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace DDO.Launcher.Dialogs;

public partial class SettingsDialogWindow : Window, INotifyPropertyChanged
{
    private string _executable = string.Empty;
    private string _accountAPI = string.Empty;
    private string _downloadIP = string.Empty;
    private string _downloadPort = string.Empty;
    private string _lobbyIP = string.Empty;
    private string _lobbyPort = string.Empty;
    private bool _requireAdmin = true;
    private bool _localMode = false;

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

    public string AccountAPI
    {
        get => _accountAPI;
        set
        {
            if (_accountAPI != value)
            {
                _accountAPI = value;
                _settings.AccountAPI = _accountAPI;
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
                _settings.DownloadIP = _downloadIP;
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
                _settings.DownloadPort = _downloadPort;
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
                _settings.LobbyIP = _lobbyIP;
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
                _settings.LobbyPort = _lobbyPort;
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

    public SettingsDialogWindow()
    {
        if (ServiceManager.Settings is null)
            NotificationProvider.Warn("log.unhandled.exception", "Settings is null");

        _settings = ServiceManager.Settings ?? new Settings();
        _executable = _settings.Executable ?? Validate.For.EmptyString();
        _accountAPI = _settings.AccountAPI ?? Validate.For.EmptyString();
        _downloadIP = _settings.DownloadIP ?? Validate.For.EmptyString();
        _downloadPort = _settings.DownloadPort ?? Validate.For.EmptyString();
        _lobbyIP = _settings.LobbyIP ?? Validate.For.EmptyString();
        _lobbyPort = _settings.LobbyPort ?? Validate.For.EmptyString();
        _requireAdmin = _settings.RequireAdmin ?? true;
        _localMode = _settings.LocalMode ?? false;

        InitializeComponent();
        DataContext = this;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        SettingsProvider.Save(_settings);
    }
}
