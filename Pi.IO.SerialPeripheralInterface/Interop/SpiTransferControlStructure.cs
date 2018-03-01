// <copyright file="SpiTransferControlStructure.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System.Runtime.InteropServices;

    /// <summary>
    /// A IOCTL structure that describes a single SPI transfer
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct SpiTransferControlStructure
    {
        /// <summary>
        /// Holds pointer to userspace buffer with transmit data, or 0. If no data is provided, zeroes are shifted out
        /// </summary>
        public ulong Tx;

        /// <summary>
        /// Holds pointer to userspace buffer for receive data, or 0
        /// </summary>
        public ulong Rx;

        /// <summary>
        /// Length of <see cref="Tx"/> and <see cref="Rx"/> buffers, in bytes
        /// </summary>
        public uint Length;

        /// <summary>
        /// Temporary override of the device's bitrate (in Hz)
        /// </summary>
        public uint Speed;

        /// <summary>
        /// If nonzero, how long to delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer
        /// </summary>
        public ushort Delay;

        /// <summary>
        /// Temporary override of the device's wordsize
        /// </summary>
        public byte BitsPerWord;

        /// <summary>
        /// Set to <c>true</c> to deselect device before starting the next transfer
        /// </summary>
        public byte ChipSelectChange;

        /// <summary>
        /// Pad
        /// </summary>
        public uint Pad;
    }
}