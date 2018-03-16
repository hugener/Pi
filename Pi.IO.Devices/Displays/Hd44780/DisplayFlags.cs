// <copyright file="DisplayFlags.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Displays.Hd44780
{
    using global::System;

    [Flags]
    internal enum DisplayFlags
    {
        None = 0,

        BlinkOff = 0,
        CursorOff = 0,
        DisplayOff = 0,

        BlinkOn = 0x01,
        CursorOn = 0x02,
        DisplayOn = 0x04,
    }
}