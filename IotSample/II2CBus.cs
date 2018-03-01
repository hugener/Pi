// <copyright file="II2CBus.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace I2C.Net
{
    public interface II2CBus
    {
        /// <summary>
        /// Writes single byte.
        /// </summary>
        /// <param name="address">Address of a destination device</param>
        /// <param name="value">The value.</param>
        void WriteByte(int address, byte value);

        /// <summary>
        /// Writes array of bytes.
        /// </summary>
        /// <remarks>Do not write more than 3 bytes at once, RPi drivers don't support this currently.</remarks>
        /// <param name="address">Address of a destination device</param>
        /// <param name="bytes">The bytes.</param>
        void WriteBytes(int address, byte[] bytes);

        /// <summary>
        /// Writes command with data.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="command">The command.</param>
        /// <param name="data">The data.</param>
        void WriteCommand(int address, byte command, byte data);

        /// <summary>
        /// Writes command with data.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="command">The command.</param>
        /// <param name="data1">The data1.</param>
        /// <param name="data2">The data2.</param>
        void WriteCommand(int address, byte command, byte data1, byte data2);

        /// <summary>
        /// Writes command with data.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="command">The command.</param>
        /// <param name="data">The data.</param>
        void WriteCommand(int address, byte command, ushort data);

        /// <summary>
        /// Reads bytes from device with passed address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <returns>The read bytes.</returns>
        byte[] ReadBytes(int address, int count);
    }
}