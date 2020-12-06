// <copyright file="I2cDeviceConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.InterIntegratedCircuit
{
    using global::System;

    /// <summary>
    /// Represents a connection to the I2C device.
    /// </summary>
    public class I2cDeviceConnection
    {
        private readonly I2cDriver driver;
        private readonly int deviceAddress;
        private readonly II2cDeviceConnectionReporter i2cDeviceConnectionReporter;

        internal I2cDeviceConnection(I2cDriver driver, int deviceAddress, II2cDeviceConnectionReporter i2CDeviceConnectionReporter = null)
        {
            this.driver = driver;
            this.deviceAddress = deviceAddress;
            this.i2cDeviceConnectionReporter = i2CDeviceConnectionReporter;
            this.i2cDeviceConnectionReporter?.SetSource(typeof(II2cDeviceConnectionReporter), this);
            this.i2cDeviceConnectionReporter?.Connect(deviceAddress);
        }

        /// <summary>
        /// Gets the device address.
        /// </summary>
        /// <value>
        /// The device address.
        /// </value>
        public int DeviceAddress => this.deviceAddress;

        /// <summary>
        /// Executes the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        public void Execute(I2CTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            this.driver.Execute(this.deviceAddress, transaction);
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void Write(params byte[] buffer)
        {
            try
            {
                this.Execute(new I2CTransaction(new I2CWriteAction(buffer)));
                this.i2cDeviceConnectionReporter?.Wrote(buffer);
            }
            catch (Exception e)
            {
                this.i2cDeviceConnectionReporter?.WriteError(e, buffer);
            }
        }

        /// <summary>
        /// Writes the specified byte.
        /// </summary>
        /// <param name="value">The value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500:Braces for multi-line statements should not share line", Justification = "False positive.")]
        public void WriteByte(byte value)
        {
            Span<byte> values = stackalloc byte[] { value };
            try
            {
                this.Execute(new I2CTransaction(new I2CWriteAction(value)));
                this.i2cDeviceConnectionReporter?.Wrote(values);
            }
            catch (Exception e)
            {
                this.i2cDeviceConnectionReporter?.WriteError(e, values);
            }
        }

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The byte count.</param>
        /// <returns>The buffer.</returns>
        public byte[] Read(int byteCount)
        {
            var readAction = new I2CReadAction(new byte[byteCount]);
            this.Execute(new I2CTransaction(readAction));
            this.i2cDeviceConnectionReporter?.Read(readAction.Buffer);

            return readAction.Buffer;
        }

        /// <summary>
        /// Reads a byte.
        /// </summary>
        /// <returns>The byte.</returns>
        public byte ReadByte()
        {
            return this.Read(1)[0];
        }
    }
}
