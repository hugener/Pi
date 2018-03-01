// <copyright file="I2CBus.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace I2C.Net
{
    using System;
    using System.IO;

    public class I2CBus : II2CBus, IDisposable
    {
        private int busHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="I2CBus"/> class.
        /// </summary>
        /// <param name="busPath">The bus path.</param>
        /// <exception cref="IOException">Thrown if the bus could not be opened.</exception>
        protected I2CBus(string busPath)
        {
            int ret = I2CNativeLib.Open(busPath, I2CNativeLib.OpenReadWrite);
            if (ret < 0)
            {
                throw new IOException(string.Format("Error opening bus '{0}'", busPath/*, UnixMarshal.GetErrorDescription(Stdlib.GetLastError())*/));
            }

            this.busHandle = ret;
        }

        /// <summary>
        /// Creates new instance of I2CBus.
        /// </summary>
        /// <param name="busPath">Path to system file associated with I2C bus.<br/>
        /// For RPi rev.1 it's usually "/dev/i2c-0",<br/>
        /// For rev.2 it's "/dev/i2c-1".</param>
        /// <returns>A new <see cref="I2CBus"/>.</returns>
        public static I2CBus Open(string busPath)
        {
            return new I2CBus(busPath);
        }

        /// <summary>
        /// Writes command with no data.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="command">The command.</param>
        public void WriteCommand(int address, byte command)
        {
            byte[] bytes = new byte[2];
            bytes[0] = command;
            bytes[1] = 0x00;
            this.WriteBytes(address, bytes);
        }

        /// <summary>
        /// Writes single byte.
        /// </summary>
        /// <param name="address">Address of a destination device</param>
        /// <param name="value">The value.</param>
        public void WriteByte(int address, byte value)
        {
            byte[] bytes = new byte[1];
            bytes[0] = value;
            this.WriteBytes(address, bytes);
        }

        /// <summary>
        /// Writes array of bytes.
        /// </summary>
        /// <param name="address">Address of a destination device</param>
        /// <param name="bytes">The bytes.</param>
        /// <exception cref="IOException">Thrown when address cannot be accessed or data cannot be written.</exception>
        /// <remarks>
        /// Do not write more than 3 bytes at once, RPi drivers don't support this currently.
        /// </remarks>
        public void WriteBytes(int address, byte[] bytes)
        {
            int ret = I2CNativeLib.Ioctl(this.busHandle, I2CNativeLib.I2CSlave, address);

            if (ret < 0)
            {
                throw new IOException(string.Format("Error accessing address '{0}'", address/*, UnixMarshal.GetErrorDescription(Stdlib.GetLastError())*/));
            }

            ret = I2CNativeLib.Write(this.busHandle, bytes, bytes.Length);

            if (ret < 0)
            {
                throw new IOException(string.Format("Error writing to address '{0}': I2C transaction failed", address));
            }
        }

        /// <summary>
        /// Writes command with data.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="command">The command.</param>
        /// <param name="data">The data.</param>
        public void WriteCommand(int address, byte command, byte data)
        {
            byte[] bytes = new byte[2];
            bytes[0] = command;
            bytes[1] = data;
            this.WriteBytes(address, bytes);
        }

        /// <summary>
        /// Writes command with data.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="command">The command.</param>
        /// <param name="data1">The data1.</param>
        /// <param name="data2">The data2.</param>
        public void WriteCommand(int address, byte command, byte data1, byte data2)
        {
            byte[] bytes = new byte[3];
            bytes[0] = command;
            bytes[1] = data1;
            bytes[2] = data2;
            this.WriteBytes(address, bytes);
        }

        /// <summary>
        /// Writes command with data.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="command">The command.</param>
        /// <param name="data">The data.</param>
        public void WriteCommand(int address, byte command, ushort data)
        {
            byte[] bytes = new byte[3];
            bytes[0] = command;
            bytes[1] = (byte)(data & 0xff);
            bytes[2] = (byte)(data >> 8);
            this.WriteBytes(address, bytes);
        }

        /// <summary>
        /// Reads bytes from device with passed address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// The read bytes.
        /// </returns>
        /// <exception cref="IOException">Thrown if the address could not be accessed or read.</exception>
        public byte[] ReadBytes(int address, int count)
        {
            byte[] buf = new byte[count];

            // int res= I2CNativeLib.ReadBytes(busHandle, address, buf, buf.Length);
            int ret = I2CNativeLib.Ioctl(this.busHandle, I2CNativeLib.I2CSlave, address);

            if (ret < 0)
            {
                throw new IOException(string.Format("Error accessing address '{0}'", address/*, UnixMarshal.GetErrorDescription(Stdlib.GetLastError())*/));
            }

            int n = I2CNativeLib.Read(this.busHandle, buf, buf.Length);

            if (n <= 0)
            {
                throw new IOException(string.Format("Error reading from address '{0}': I2C transaction failed", address));
            }

            if (n < count)
            {
                Array.Resize(ref buf, n);
            }

            return buf;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // disposing managed resouces
            }

            if (this.busHandle != 0)
            {
                I2CNativeLib.Close(this.busHandle);
                this.busHandle = 0;
            }
        }
    }
}