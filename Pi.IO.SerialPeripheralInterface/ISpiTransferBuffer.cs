// <copyright file="ISpiTransferBuffer.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;
    using Pi.IO.Interop;

    /// <summary>
    /// A transfer buffer used to read from / write to the SPI bus.
    /// </summary>
    public interface ISpiTransferBuffer : IDisposable
    {
        /// <summary>
        /// Gets or sets the temporary override of the device's wordsize.
        /// </summary>
        /// <value>
        /// The bits per word.
        /// </value>
        byte BitsPerWord { get; set; }

        /// <summary>
        /// Gets or sets the temporary override of the device's bitrate (in Hz).
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        uint Speed { get; set; }

        /// <summary>
        /// Gets or sets the delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer.
        /// </summary>
        /// <value>
        /// The delay.
        /// </value>
        ushort Delay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [chip select change].
        /// </summary>
        /// <value>
        ///   <c>true</c> if device is delected before starting next transfer; otherwise, <c>false</c>.
        /// </value>
        bool ChipSelectChange { get; set; }

        /// <summary>
        /// Gets or sets the Pad.
        /// </summary>
        /// <value>
        /// The pad.
        /// </value>
        uint Pad { get; set; }

        /// <summary>
        /// Gets the spi transfer mode (read and/or write). <see cref="SpiTransferMode" />.
        /// </summary>
        /// <value>
        /// The transfer mode.
        /// </value>
        SpiTransferMode TransferMode { get; }

        /// <summary>
        /// Gets the length of <see cref="Tx" /> and <see cref="Rx" /> buffers, in bytes.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        int Length { get; }

        /// <summary>
        /// Gets the pointer to userspace buffer with transmit data, or <c>null</c>. If no data is provided, zeroes are shifted out.
        /// </summary>
        /// <value>
        /// The tx.
        /// </value>
        IMemory Tx { get; }

        /// <summary>
        /// Gets the pointer to userspace buffer for receive data, or <c>null</c>.
        /// </summary>
        /// <value>
        /// The rx.
        /// </value>
        IMemory Rx { get; }

        /// <summary>
        /// Gets the IOCTL structure that contains control information for a single SPI transfer.
        /// </summary>
        /// <value>
        /// The control structure.
        /// </value>
        SpiTransferControlStructure ControlStructure { get; }
    }
}