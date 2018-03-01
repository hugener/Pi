// <copyright file="Pcf8574Pin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Expanders.Pcf8574
{
    /// <summary>
    /// Enum for the pins of <see cref="Pcf8574I2CConnection"/>.
    /// </summary>
    public enum Pcf8574Pin
    {
        /// <summary>
        /// The p0
        /// </summary>
        P0 = 0x01,

        /// <summary>
        /// The p1
        /// </summary>
        P1 = 0x02,

        /// <summary>
        /// The p2
        /// </summary>
        P2 = 0x04,

        /// <summary>
        /// The p3
        /// </summary>
        P3 = 0x08,

        /// <summary>
        /// The p4
        /// </summary>
        P4 = 0x10,

        /// <summary>
        /// The p5
        /// </summary>
        P5 = 0x20,

        /// <summary>
        /// The p6
        /// </summary>
        P6 = 0x40,

        /// <summary>
        /// The p7
        /// </summary>
        P7 = 0x80
    }
}