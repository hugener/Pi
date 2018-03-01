﻿// <copyright file="BH1750Connection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Sensors.Light
{
    using Pi.IO.InterIntegratedCircuit;
    using Pi.System.Threading;
    using Pi.Timers;

    /// <summary>
    /// Represents a connection to a Bh1750 light sensor.
    /// </summary>
    public class Bh1750Connection
    {
        private readonly IThread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bh1750Connection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="thread">The thread.</param>
        public Bh1750Connection(I2cDeviceConnection connection, IThread thread)
        {
            this.thread = thread;
            this.Connection = connection;
        }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public I2cDeviceConnection Connection { get; set; }

        /// <summary>
        /// Sets the off.
        /// </summary>
        public void SetOff()
        {
            this.Connection.Write(0x00);
        }

        /// <summary>
        /// Sets the on.
        /// </summary>
        public void SetOn()
        {
            this.Connection.Write(0x01);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.Connection.Write(0x07);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The data.</returns>
        public double GetData()
        {
            this.Connection.Write(0x10);
            this.thread.Sleep(TimeSpanUtility.FromMicroseconds(150 * 1000));
            byte[] readBuf = this.Connection.Read(2);

            var valf = readBuf[0] << 8;
            valf |= readBuf[1];
            return valf / 1.2 * (69 / 69) / 1;

            // var valf = ((readBuf[0] << 8) | readBuf[1]) / 1.2;
            // return valf;

            // return Math.Round(valf / (2 * 1.2), 2);
        }
    }
}
