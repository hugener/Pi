// <copyright file="EntryModeFlags.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Displays.Hd44780
{
    using global::System;

    [Flags]
    internal enum EntryModeFlags
    {
        None = 0,

        EntryRight = 0,
        EntryShiftDecrement = 0,

        EntryShiftIncrement = 0x01,
        EntryLeft = 0x02,
    }
}