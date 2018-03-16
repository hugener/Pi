// <copyright file="Mcp23008Pin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Expanders.Mcp23008
{
    /// <summary>
    /// Enum for the pins of <see cref="Mcp23008Device"/>.
    /// </summary>
    public enum Mcp23008Pin
    {
        /// <summary>
        /// The pin0
        /// </summary>
        Pin0 = 0x0001,

        /// <summary>
        /// The pin1
        /// </summary>
        Pin1 = 0x0002,

        /// <summary>
        /// The pin2
        /// </summary>
        Pin2 = 0x0004,

        /// <summary>
        /// The pin3
        /// </summary>
        Pin3 = 0x0008,

        /// <summary>
        /// The pin4
        /// </summary>
        Pin4 = 0x0010,

        /// <summary>
        /// The pin5
        /// </summary>
        Pin5 = 0x0020,

        /// <summary>
        /// The pin6
        /// </summary>
        Pin6 = 0x0040,

        /// <summary>
        /// The pin7
        /// </summary>
        Pin7 = 0x0080,
    }
}
