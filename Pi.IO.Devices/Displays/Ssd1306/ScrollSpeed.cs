// <copyright file="ScrollSpeed.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Displays.Ssd1306
{
    using global::System;

    /// <summary>
    /// Enum for the scroll speed.
    /// </summary>
    [Flags]
    public enum ScrollSpeed : byte
    {
        /// <summary>
        /// The f2
        /// </summary>
        F2 = 0x7,

        /// <summary>
        /// The f3
        /// </summary>
        F3 = 0x4,

        /// <summary>
        /// The f4
        /// </summary>
        F4 = 0x5,

        /// <summary>
        /// The f5
        /// </summary>
        F5 = 0x0,

        /// <summary>
        /// The F25
        /// </summary>
        F25 = 0x6,

        /// <summary>
        /// The F64
        /// </summary>
        F64 = 0x1,

        /// <summary>
        /// The F128
        /// </summary>
        F128 = 0x2,

        /// <summary>
        /// The F256
        /// </summary>
        F256 = 0x3,
    }
}