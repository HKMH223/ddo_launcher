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
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using DDO.Launcher.Base.Helpers;
using DDO.Launcher.Base.Managers;
using DDO.Launcher.Base.Models;
using DDO.Launcher.Base.Providers;
using DDO.Launcher.Base.Services;
using DDO.Launcher.Dialogs;
using MiniCommon.BuildInfo;
using MiniCommon.IO;
using MiniCommon.IO.Helpers;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace DDO.Launcher;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private string _username = string.Empty;
    private string _password = string.Empty;
    private bool _remember = false;

    private readonly DDOAccountService _accountService;
    private readonly Settings _settings;

    public new event PropertyChangedEventHandler? PropertyChanged;

    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                _settings.Account = _username;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                _settings.Password = _password;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
            }
        }
    }

    public bool Remember
    {
        get => _remember;
        set
        {
            if (_remember != value)
            {
                _remember = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Remember)));
            }
        }
    }

    public MainWindow()
    {
        if (ServiceManager.Settings is null)
            NotificationProvider.Warn("log.unhandled.exception", "Settings is null");

        _settings = ServiceManager.Settings ?? new Settings();
        _accountService = new(_settings);

        InitializeComponent();
        DataContext = this;
    }

    private void Version_Initialized(object sender, EventArgs e)
    {
        if (sender is TextBlock { Text: not null } block)
            block.Text = $"v{ApplicationConstants.CurrentVersion}";
    }

    private void UsernameTextBox_Initialized(object sender, EventArgs e)
    {
        if (_settings.Account != string.Empty && _settings.Password != string.Empty)
        {
            Username = _settings.Account ?? Validate.For.EmptyString();
            Password = _settings.Password ?? Validate.For.EmptyString();
        }
    }

    private void RememberMe_Unchecked(object sender, RoutedEventArgs e)
    {
        Settings settings = _settings;
        settings.Account = string.Empty;
        settings.Password = string.Empty;
        SettingsProvider.Save(settings);
    }

    private void ModdingButton_Click(object sender, RoutedEventArgs e)
    {
        new ModdingDialog(LocalizationProvider.Translate("avalonia.title.modding"), this).Show();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        new SettingsDialog(LocalizationProvider.Translate("avalonia.title.settings"), this).Show();
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        _settings.Account = _username;
        _settings.Password = _password;

        if (_remember)
            SettingsProvider.Save(_settings);

        IsHitTestVisible = false;
        await RegisterTask();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        _settings.Account = _username;
        _settings.Password = _password;

        if (_remember)
            SettingsProvider.Save(_settings);

        IsHitTestVisible = false;
        await LoginTask();
    }

    /// <summary>
    /// Execute the registration request asynchronously to not lock main thread.
    /// </summary>
    private async Task RegisterTask()
    {
        bool registrationSuccess = await Task.Run(_accountService.Register);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            IsHitTestVisible = true;
            if (!registrationSuccess)
            {
                new MessageDialog(
                    LocalizationProvider.Translate("avalonia.title.error"),
                    LocalizationProvider.Translate("ddo.register.failed"),
                    this
                ).Show();
                return;
            }

            new MessageDialog(
                LocalizationProvider.Translate("avalonia.title.confirm"),
                LocalizationProvider.Translate("ddo.register.success"),
                this
            ).Show();
        });
    }

    /// <summary>
    /// Execute the login request asynchronously to not lock main thread.
    /// </summary>
    private async Task LoginTask()
    {
        bool loginSuccess = await Task.Run(_accountService.Login);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            IsHitTestVisible = true;
            if (!loginSuccess)
            {
                new MessageDialog(
                    LocalizationProvider.Translate("avalonia.title.error"),
                    LocalizationProvider.Translate("ddo.login.failed"),
                    this
                ).Show();
                return;
            }

            if (!WindowsAdminHelper.IsAdmin() && _settings.RequireAdmin == true)
            {
                new MessageDialog(
                    LocalizationProvider.Translate("avalonia.title.error"),
                    LocalizationProvider.Translate("error.admin.required"),
                    this
                ).Show();
                return;
            }

            DDOLauncher.Launch(_settings!, _accountService.Token ?? Validate.For.EmptyString(), VFS.FileSystem.Cwd);
        });
    }
}
