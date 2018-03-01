// <copyright file="Mcp4822SpiConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Converters.Mcp4822
{
    using global::System;
    using SerialPeripheralInterface;

    /// <summary>
    /// Represents a SPI connection to a MCP4802/4812/4822 DAC.
    /// </summary>
    /// <remarks>See http://ww1.microchip.com/downloads/en/DeviceDoc/22249A.pdf for specifications.</remarks>
    public class Mcp4822SpiConnection : IDisposable
    {
        private readonly SpiConnection spiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp4822SpiConnection" /> class.
        /// </summary>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="slaveSelectPin">The slave select pin.</param>
        /// <param name="mosiPin">The mosi pin.</param>
        public Mcp4822SpiConnection(IOutputBinaryPin clockPin, IOutputBinaryPin slaveSelectPin, IOutputBinaryPin mosiPin)
        {
            this.spiConnection = new SpiConnection(clockPin, slaveSelectPin, null, mosiPin, Endianness.LittleEndian);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this.spiConnection.Close();
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="data">The data.</param>
        public void Write(Mcp4822Channel channel, AnalogValue data)
        {
            using (this.spiConnection.SelectSlave())
            {
                var value = (uint)(data.Relative * 0xFFF);
                if (value > 0xFFF)
                {
                    value = 0xFFF;
                }

                // Set active channel
                this.spiConnection.Write(channel == Mcp4822Channel.ChannelB);

                // Ignored bit
                this.spiConnection.Synchronize();

                // Select 1x Gain
                this.spiConnection.Write(true);

                // Active mode operation
                this.spiConnection.Write(true);

                // Write 12 bits data (some lower bits are ignored by MCP4802/4812
                this.spiConnection.Write(value, 12);
            }
        }
    }
}