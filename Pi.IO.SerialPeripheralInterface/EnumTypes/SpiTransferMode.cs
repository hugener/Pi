// <copyright file="SpiTransferMode.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;

    /// <summary>
    /// Selects if data should be read, written or both.
    /// </summary>
    [Flags]
    public enum SpiTransferMode
    {
        /// <summary>
        /// Write data to the chip.
        /// </summary>
        Write = 1,

        /// <summary>
        /// Read data from the chip.
        /// </summary>
        Read = 2,

        /// <summary>
        /// Write and read data simultaneously.
        /// </summary>
        ReadWrite = 3,
    }
}