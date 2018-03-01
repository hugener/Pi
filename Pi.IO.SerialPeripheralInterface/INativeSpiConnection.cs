// <copyright file="INativeSpiConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;

    /// <summary>
    /// Native SPI connection that communicates with Linux's userspace SPI driver (e.g. /dev/spidev0.0) using IOCTL.
    /// </summary>
    public interface INativeSpiConnection : IDisposable
    {
        /// <summary>
        /// Gets the delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer.
        /// </summary>
        ushort Delay { get; }

        /// <summary>
        /// Gets the maximum clock speed in Hz.
        /// </summary>
        uint MaxSpeed { get; }

        /// <summary>
        /// Gets the SPI mode
        /// </summary>
        SpiMode Mode { get; }

        /// <summary>
        /// Gets the device's wordsize
        /// </summary>
        byte BitsPerWord { get; }

        /// <summary>
        /// Sets the <see cref="INativeSpiConnection.Delay"/>.
        /// </summary>
        /// <param name="delayInMicroSeconds">Delay in µsec.</param>
        void SetDelay(ushort delayInMicroSeconds);

        /// <summary>
        /// Sets the maximum clock speed.
        /// </summary>
        /// <param name="maxSpeedInHz">The speed in Hz</param>
        void SetMaxSpeed(uint maxSpeedInHz);

        /// <summary>
        /// Sets the device's wordsize <see cref="INativeSpiConnection.BitsPerWord"/>.
        /// </summary>
        /// <param name="wordSize">Bits per word</param>
        void SetBitsPerWord(byte wordSize);

        /// <summary>
        /// Sets the <see cref="SpiMode"/>.
        /// </summary>
        /// <param name="spiMode">SPI mode</param>
        void SetSpiMode(SpiMode spiMode);

        /// <summary>
        /// Creates a transfer buffer of the given <see paramref="sizeInBytes"/> and copies the connection settings to it.
        /// </summary>
        /// <param name="sizeInBytes">Memory size in bytes.</param>
        /// <param name="transferMode">The transfer mode.</param>
        /// <returns>The requested transfer buffer.</returns>
        ISpiTransferBuffer CreateTransferBuffer(int sizeInBytes, SpiTransferMode transferMode);

        /// <summary>
        /// Creates transfer buffers for <paramref name="numberOfMessages"/>.
        /// </summary>
        /// <param name="numberOfMessages">The number of messages to send.</param>
        /// <param name="messageSizeInBytes">Message size in bytes.</param>
        /// <param name="transferMode">The transfer mode.</param>
        /// <returns>The requested transfer buffer collection.</returns>
        ISpiTransferBufferCollection CreateTransferBufferCollection(
            int numberOfMessages,
            int messageSizeInBytes,
            SpiTransferMode transferMode);

        /// <summary>
        /// Starts the SPI data transfer.
        /// </summary>
        /// <param name="buffer">The transfer buffer that contains data to be send and/or the received data.</param>
        /// <returns>An <see cref="int"/> that contains the result of the transfer operation.</returns>
        int Transfer(ISpiTransferBuffer buffer);

        /// <summary>
        /// Starts the SPI data transfer.
        /// </summary>
        /// <param name="transferBuffers">The transfer buffers that contain data to be send and/or the received data.</param>
        /// <returns>An <see cref="int"/> that contains the result of the transfer operation.</returns>
        int Transfer(ISpiTransferBufferCollection transferBuffers);
    }
}