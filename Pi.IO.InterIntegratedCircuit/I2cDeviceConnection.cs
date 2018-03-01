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

        internal I2cDeviceConnection(I2cDriver driver, int deviceAddress)
        {
            this.driver = driver;
            this.deviceAddress = deviceAddress;
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
                throw new ArgumentNullException("transaction");
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Execute error {0}", e.Message);
            }
        }

        /// <summary>
        /// Writes the specified byte.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteByte(byte value)
        {
            try
            {
                this.Execute(new I2CTransaction(new I2CWriteAction(value)));
            }
            catch (Exception e)
            {
                Console.WriteLine("Execute error WriteByte {0}", e.Message);
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
