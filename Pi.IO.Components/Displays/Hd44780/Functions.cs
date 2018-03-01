// <copyright file="Functions.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Displays.Hd44780
{
    using global::System;

    [Flags]
    internal enum Functions
    {
        None = 0,

        Matrix5X8 = 0,
        Matrix5X10 = 0x04,

        OneLine = 0,
        TwoLines = 0x08,

        Data4Bits = 0x0,
        Data8Bits = 0x10,
    }
}