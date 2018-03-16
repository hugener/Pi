// <copyright file="Command.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Displays.Hd44780
{
    internal enum Command
    {
        ClearDisplay = 0x01,
        ReturnHome = 0x02,
        SetEntryModeFlags = 0x04,
        SetDisplayFlags = 0x08,
        MoveCursor = 0x10,
        SetFunctions = 0x20,
        SetCgRamAddr = 0x40,

        // SetDDRamAddr = 0x80
    }
}