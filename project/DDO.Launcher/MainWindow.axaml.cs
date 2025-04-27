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
using DDO.Launcher.Base;
using DDO.Launcher.Base.Helpers;
using DDO.Launcher.Base.Models;
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

#pragma warning disable RCS1043
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private string _username = string.Empty;
    private string _password = string.Empty;
    private bool _remember;

    private readonly DDOAccountService _accountService;
    private readonly Settings _runtimeSettings;

    public new event PropertyChangedEventHandler? PropertyChanged;

    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                _runtimeSettings.Account = _username;
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
                _runtimeSettings.Password = _password;
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
        if (RuntimeManager.RuntimeSettings is null)
            LogProvider.Warn("log.unhandled.exception", "Settings is null");

        _runtimeSettings = RuntimeManager.RuntimeSettings ?? new Settings();
        _accountService = new(_runtimeSettings);

        InitializeComponent();
        DataContext = this;
    }

    private static void Version_Initialized(object sender, EventArgs e)
    {
        if (sender is TextBlock { Text: not null } block)
            block.Text = $"v{ApplicationConstants.CurrentVersion}";
    }

    private void UsernameTextBox_Initialized(object sender, EventArgs e)
    {
        if (_runtimeSettings.Account != string.Empty && _runtimeSettings.Password != string.Empty)
        {
            Username = _runtimeSettings.Account ?? Validate.For.EmptyString();
            Password = _runtimeSettings.Password ?? Validate.For.EmptyString();
        }
    }

    private void RememberMe_Unchecked(object sender, RoutedEventArgs e)
    {
        Settings runtimeSettings = _runtimeSettings;
        runtimeSettings.Account = string.Empty;
        runtimeSettings.Password = string.Empty;
        RuntimeManager.SettingsManager?.Save(runtimeSettings);
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
        _runtimeSettings.Account = _username;
        _runtimeSettings.Password = _password;

        if (_remember)
            RuntimeManager.SettingsManager?.Save(_runtimeSettings);

        IsHitTestVisible = false;
        await RegisterTask();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        _runtimeSettings.Account = _username;
        _runtimeSettings.Password = _password;

        if (_remember)
            RuntimeManager.SettingsManager?.Save(_runtimeSettings);

        IsHitTestVisible = false;
        await LoginTask();
    }

    /// <summary>
    /// Execute the registration request asynchronously to not lock main thread.
    /// </summary>
    private async Task RegisterTask()
    {
        bool registrationSuccess = await TaskManager.Run(_accountService.Register);

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
        bool loginSuccess = await TaskManager.Run(_accountService.Login);

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

            if (!WindowsAdminHelper.IsAdmin() && _runtimeSettings.RequireAdmin == true)
            {
                new MessageDialog(
                    LocalizationProvider.Translate("avalonia.title.error"),
                    LocalizationProvider.Translate("error.admin.required"),
                    this
                ).Show();
                return;
            }

            DDOLauncher.Launch(
                _runtimeSettings!,
                _accountService.Token ?? Validate.For.EmptyString(),
                VFS.FileSystem.Cwd
            );
        });
    }
}
