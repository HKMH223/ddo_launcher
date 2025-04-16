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
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using DDO.Launcher.Base.Managers;
using DDO.ModManager.Base.NativePC.Helpers;
using DDO.ModManager.Base.NativePC.Models;
using DDO.ModManager.Base.NativePC.Providers;
using MiniCommon.BuildInfo;
using MiniCommon.Extensions;
using MiniCommon.IO;
using MiniCommon.IO.Enums;
using MiniCommon.IO.Helpers;
using MiniCommon.Logger.Enums;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Validators;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace DDO.Launcher.Dialogs;

public partial class ModdingDialogWindow : Window, INotifyPropertyChanged
{
    private string _game = string.Empty;

    public new event PropertyChangedEventHandler? PropertyChanged;

    public string Game
    {
        get => _game;
        set
        {
            if (_game != value)
            {
                _game = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Game)));
            }
        }
    }

    public ModdingDialogWindow()
    {
        if (ServiceManager.Settings is null)
            NotificationProvider.Warn("log.unhandled.exception", "Settings is null");

        InitializeComponent();
        DataContext = this;
    }

    private void DeployButton_Click(object sender, RoutedEventArgs e)
    {
        IsHitTestVisible = false;
        Topmost = false;
        Task.Run(DeployTask)
            .ContinueWith(_ =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    IsHitTestVisible = true;
                    Topmost = true;
                });
            });
    }

    private Task DeployTask()
    {
        if (Validate.For.IsNullOrWhiteSpace([_game]))
            return Task.CompletedTask;

        (string? gamePath, string expectedGamePath) = JsonExtensionHelper.MaybeJsonWithComments(
            VFS.GetRelativePath(
                    VFS.FileSystem.Cwd,
                    VFS.Combine(
                        AssemblyConstants.DataDirectory,
                        "games",
                        _game + JsonExtensionHelper.ToString(JsonExtensionType.Default)
                    )
                )
                .NormalizePath()
        );
        (string? rulePath, string expectedRulePath) = JsonExtensionHelper.MaybeJsonWithComments(
            VFS.GetRelativePath(
                    VFS.FileSystem.Cwd,
                    VFS.Combine(
                        AssemblyConstants.DataDirectory,
                        "user",
                        _game + JsonExtensionHelper.ToString(JsonExtensionType.Default)
                    )
                )
                .NormalizePath()
        );

        if (gamePath is null)
        {
            NotificationProvider.Error("error.readfile", expectedGamePath);
            return Task.CompletedTask;
        }

        if (rulePath is null)
        {
            NotificationProvider.Error("error.readfile", expectedRulePath);
            return Task.CompletedTask;
        }

        NtPcGame? game = NtPcGame.Read(gamePath);
        if (Validate.For.IsNull(game))
            return Task.CompletedTask;

        NtPcRules? rules = NtPcRules.Read(rulePath);
        if (Validate.For.IsNull(game))
            return Task.CompletedTask;

        if (Validate.For.IsNull(game!.Deploy, NativeLogLevel.Fatal))
            return Task.CompletedTask;

        if (
            Validate.For.IsNullOrWhiteSpace(
                [game.Deploy!.Mods, game.Deploy.Temp, game.Deploy.Output],
                NativeLogLevel.Fatal
            )
        )
        {
            return Task.CompletedTask;
        }

        string modPath = PathHelper.MaybeCwd(game.Deploy.Mods!, VFS.FileSystem.Cwd).NormalizePath();
        string tempPath = PathHelper.MaybeCwd(game.Deploy.Temp!, VFS.FileSystem.Cwd).NormalizePath();
        string outputPath = PathHelper.MaybeCwd(game.Deploy.Output!, VFS.FileSystem.Cwd).NormalizePath();

        if (!VFS.Exists(modPath))
        {
            NotificationProvider.Error("error.readfile", modPath);
            return Task.CompletedTask;
        }

        ExtractHelper.Extract(modPath, tempPath, game);
        NtPcProvider.DeleteDirectory(outputPath);
        NtPcProvider.Deploy(tempPath, outputPath, game, rules!);
        NtPcProvider.DeleteDirectory(tempPath);

        return Task.CompletedTask;
    }
}
