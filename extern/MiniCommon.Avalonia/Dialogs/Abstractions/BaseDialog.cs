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
using System.Threading.Tasks;
using Avalonia.Controls;
using MiniCommon.Avalonia.Dialogs.Interfaces;
using MiniCommon.Providers;
using MiniCommon.Validation;
using MiniCommon.Validation.Operators;
using Window = Avalonia.Controls.Window;

namespace MiniCommon.Avalonia.Dialogs.Abstractions;

public class BaseDialog : IBaseDialog
{
    private Window _dialog;
    private Window _window;
    public TaskCompletionSource<bool> Confirmed = new();

    public Window Dialog
    {
        get => _dialog;
        private set
        {
            if (_dialog != value)
                _dialog = value;
        }
    }

    public Window Window
    {
        get => _window;
        private set
        {
            if (_window != value)
                _window = value;
        }
    }

    public BaseDialog(Window dialog, Window window)
    {
        _dialog = dialog;
        _window = window;
        _dialog.Closed += Close;
    }

    /// <inheritdoc/>
    public virtual T FindControl<T>(string name)
        where T : Control
    {
        return _dialog.FindControl<T>(name) ?? throw new NullReferenceException();
    }

    /// <inheritdoc/>
    public virtual void Show()
    {
        NotificationProvider.Info(
            "avalonia.open.dialog",
            _dialog.Title ?? Validate.For.EmptyString()
        );
        _window.IsHitTestVisible = false;
        _dialog.Show();
    }

    /// <inheritdoc/>
    public virtual void Close(object? sender, EventArgs e)
    {
        _window.IsHitTestVisible = true;
        _dialog.Close();
    }

    /// <inheritdoc/>
    public virtual void OnClick(object? sender, EventArgs e)
    {
        TextBlock? confirmButton = _dialog.FindControl<TextBlock>("Content");

        if (sender == confirmButton)
        {
            Confirmed.SetResult(true);
        }
        else
        {
            Confirmed.SetResult(false);
        }

        NotificationProvider.Info(
            "avalonia.close.dialog",
            _dialog.Title ?? Validate.For.EmptyString()
        );
        _window.IsHitTestVisible = true;
        _dialog.Close();
    }

    /// <inheritdoc/>
    public virtual void IsEnabled(bool value)
    {
        _dialog.IsEnabled = value;
    }

    /// <inheritdoc/>
    public virtual void IsHitTestVisible(bool value)
    {
        _dialog.IsHitTestVisible = value;
    }

    /// <inheritdoc/>
    public virtual void Topmost(bool value)
    {
        _dialog.Topmost = value;
    }
}
