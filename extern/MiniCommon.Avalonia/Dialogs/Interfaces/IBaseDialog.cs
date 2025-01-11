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
using Avalonia.Controls;

namespace MiniCommon.Avalonia.Dialogs.Interfaces;

public interface IBaseDialog
{
    /// <summary>
    /// Finds an Avalonia.Control of type T based on the given name.
    /// </summary>
    public abstract T FindControl<T>(string name)
        where T : Control;

    /// <summary>
    /// Explicitly show the dialog window.
    /// </summary>
    public abstract void Show();

    /// <summary>
    /// Explicitly close the dialog window.
    /// </summary>
    public abstract void Close(object? sender, EventArgs e);

    /// <summary>
    /// OnClick button event.
    /// </summary>
    public abstract void OnClick(object? sender, EventArgs e);

    /// <summary>
    /// Enable or disable the dialog window.
    /// </summary>
    public abstract void IsEnabled(bool value);

    /// <summary>
    /// Enable or disable hit test on the dialog window.
    /// </summary>
    public abstract void IsHitTestVisible(bool value);

    /// <summary>
    /// Set if dialog window should be the topmost window.
    /// </summary>
    public abstract void Topmost(bool value);
}
