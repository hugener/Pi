// <copyright file="SpiConnectionSettings.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    /// <summary>
    /// SPI connection parameters
    /// </summary>
    public class SpiConnectionSettings
    {
        private uint maxSpeed = 5000000;
        private ushort delay;
        private byte bitsPerWord = 8;

        /// <summary>
        /// Gets or sets the clock speed in Hz
        /// </summary>
        public uint MaxSpeed
        {
            get => this.maxSpeed;
            set => this.maxSpeed = value;
        }

        /// <summary>
        /// Gets or sets the delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer
        /// </summary>
        public ushort Delay
        {
            get => this.delay;
            set => this.delay = value;
        }

        /// <summary>
        /// Gets or sets the device's word size
        /// </summary>
        public byte BitsPerWord
        {
            get => this.bitsPerWord;
            set => this.bitsPerWord = value;
        }

        /// <summary>
        /// Gets or sets the SPI mode
        /// </summary>
        public SpiMode Mode { get; set; }
    }
}