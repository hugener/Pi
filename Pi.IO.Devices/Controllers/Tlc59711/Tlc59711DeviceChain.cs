// <copyright file="Tlc59711DeviceChain.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.Tlc59711
{
    using global::System;
    using Pi.IO.SerialPeripheralInterface;

    /// <summary>
    /// A connection the one or more TLC59711 devices.
    /// </summary>
    public class Tlc59711DeviceChain : ITlc59711DeviceChain
    {
        private const int BitsPerWord = 8;
        private const SpiMode SpiMode0 = SpiMode.Mode0;
        private const int Speed = 10000000; // Spec say max 10Mhz (RaspberryPI will only use about ~7Mhz)
        private const int Delay = 20;

        private readonly INativeSpiConnection connection;
        private readonly ISpiTransferBuffer transferBuffer;
        private readonly Tlc59711Cluster deviceCluster;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tlc59711DeviceChain"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="initializeWithDefault">if set to <c>true</c> [initialize with default].</param>
        /// <param name="numberOfDevices">The number of devices.</param>
        /// <exception cref="ArgumentNullException">connection.</exception>
        /// <exception cref="ArgumentOutOfRangeException">numberOfDevices - You need at least one device.</exception>
        public Tlc59711DeviceChain(INativeSpiConnection connection, bool initializeWithDefault, int numberOfDevices)
        {
            if (connection is null)
            {
                throw new ArgumentNullException("connection");
            }

            if (numberOfDevices <= 0)
            {
                throw new ArgumentOutOfRangeException("numberOfDevices", "You need at least one device.");
            }

            this.connection = connection;

            if (initializeWithDefault)
            {
                connection.SetBitsPerWord(BitsPerWord);
                connection.SetSpiMode(SpiMode0);
                connection.SetMaxSpeed(Speed);
                connection.SetDelay(Delay);
            }

            var requiredMemorySize = numberOfDevices * Tlc59711Device.CommandSize;
            this.transferBuffer = connection.CreateTransferBuffer(requiredMemorySize, SpiTransferMode.Write);

            this.deviceCluster = new Tlc59711Cluster(this.transferBuffer.Tx, numberOfDevices);
        }

        /// <summary>
        /// Gets a chained cluster of Adafruit's 12-channel 16bit PWM/LED driver TLC59711.
        /// </summary>
        public ITlc59711Cluster Devices => this.deviceCluster;

        /// <summary>
        /// Creates a TLC59711 command and sends it to the first device using the SPI bus.
        /// </summary>
        public void Update()
        {
            this.connection.Transfer(this.transferBuffer);
        }

        /// <summary>
        /// Releases all managed resources. The SPI connection will be closed.
        /// </summary>
        /// <filterpriority>2.</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all managed resources.
        /// </summary>
        /// <param name="disposing">If <c>true</c>, all managed resources including the SPI connection will be released/closed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.transferBuffer.Dispose();
                this.connection.Dispose();
            }
        }
    }
}