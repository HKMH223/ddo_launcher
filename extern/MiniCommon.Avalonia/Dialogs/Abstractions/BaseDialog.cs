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
using MiniCommon.Validation.Exceptions;
using MiniCommon.Validation.Operators;
using Window = Avalonia.Controls.Window;

namespace MiniCommon.Avalonia.Dialogs.Abstractions;

public class BaseDialog : IBaseDialog
{
    public TaskCompletionSource<bool> Confirmed { get; } = new();

    public Window Dialog { get; }

    public Window Window { get; }

    public BaseDialog(Window dialog, Window window)
    {
        Dialog = dialog;
        Window = window;
        Dialog.Closed += Close;
    }

    /// <inheritdoc/>
    public virtual T FindControl<T>(string name)
        where T : Control
    {
        return Dialog.FindControl<T>(name) ?? throw new ObjectValidationException(nameof(name));
    }

    /// <inheritdoc/>
    public virtual void Show()
    {
        LogProvider.Info("avalonia.open.dialog", Dialog.Title ?? Validate.For.EmptyString());
        Window.IsHitTestVisible = false;
        Dialog.Show();
    }

    /// <inheritdoc/>
    public virtual void Close(object? sender, EventArgs e)
    {
        Window.IsHitTestVisible = true;
        Dialog.Close();
    }

    /// <inheritdoc/>
    public virtual void OnClick(object? sender, EventArgs e)
    {
        TextBlock? confirmButton = Dialog.FindControl<TextBlock>("Content");

        if (sender == confirmButton)
        {
            Confirmed.SetResult(true);
        }
        else
        {
            Confirmed.SetResult(false);
        }

        LogProvider.Info("avalonia.close.dialog", Dialog.Title ?? Validate.For.EmptyString());
        Window.IsHitTestVisible = true;
        Dialog.Close();
    }

    /// <inheritdoc/>
    public virtual void IsEnabled(bool value)
    {
        Dialog.IsEnabled = value;
    }

    /// <inheritdoc/>
    public virtual void IsHitTestVisible(bool value)
    {
        Dialog.IsHitTestVisible = value;
    }

    /// <inheritdoc/>
    public virtual void Topmost(bool value)
    {
        Dialog.Topmost = value;
    }
}
