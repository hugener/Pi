// <copyright file="Mcp3002Device.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Converters.Mcp3002
{
    using global::Pi.IO.SerialPeripheralInterface;
    using global::System;

    /// <summary>
    /// Represents a connection to MCP3002 ADC converter.
    /// </summary>
    public class Mcp3002Device : IDisposable
    {
        private readonly SpiConnection spiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp3002Device"/> class.
        /// </summary>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="slaveSelectPin">The slave select pin.</param>
        /// <param name="misoPin">The miso pin.</param>
        /// <param name="mosiPin">The mosi pin.</param>
        public Mcp3002Device(IOutputBinaryPin clockPin, IOutputBinaryPin slaveSelectPin, IInputBinaryPin misoPin, IOutputBinaryPin mosiPin)
        {
            this.spiConnection = new SpiConnection(clockPin, slaveSelectPin, misoPin, mosiPin, Endianness.LittleEndian);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Reads the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>The value</returns>
        public AnalogValue Read(Mcp3002Channel channel)
        {
            using (this.spiConnection.SelectSlave())
            {
                // Start bit
                this.spiConnection.Write(true);

                // Channel is single-ended
                this.spiConnection.Write(true);

                // Channel Id
                this.spiConnection.Write((byte)channel, 1);

                // Let one clock to sample
                this.spiConnection.Synchronize();

                // Read 10 bits
                var data = (int)this.spiConnection.Read(10);

                return new AnalogValue(data, 0x3FF);
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this.spiConnection.Close();
        }
    }
}