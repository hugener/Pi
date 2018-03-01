// <copyright file="Endianness.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    /// <summary>
    /// Represents the endianness of a SPI communication.
    /// </summary>
    public enum Endianness
    {
        /// <summary>
        /// The communication is little endian.
        /// </summary>
        LittleEndian,

        /// <summary>
        /// The communication is big endian.
        /// </summary>
        BigEndian
    }
}