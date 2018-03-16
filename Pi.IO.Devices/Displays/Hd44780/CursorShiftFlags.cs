// <copyright file="CursorShiftFlags.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Displays.Hd44780
{
    using global::System;

    [Flags]
    internal enum CursorShiftFlags
    {
        None = 0,

        CursorMove = 0,
        MoveLeft = 0,

        MoveRight = 0x04,
        DisplayMove = 0x08,
    }
}