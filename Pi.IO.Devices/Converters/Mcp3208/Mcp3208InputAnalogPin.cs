// <copyright file="Mcp3208InputAnalogPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Converters.Mcp3208
{
    /// <summary>
    /// Analog input pin for the <see cref="Mcp3208Device"/>.
    /// </summary>
    /// <seealso cref="Pi.IO.IInputAnalogPin" />
    public class Mcp3208InputAnalogPin : IInputAnalogPin
    {
        private readonly Mcp3208Device connection;
        private readonly Mcp3208Channel channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp3208InputAnalogPin" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        public Mcp3208InputAnalogPin(Mcp3208Device connection, Mcp3208Channel channel)
        {
            this.connection = connection;
            this.channel = channel;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Reads the value of the pin.
        /// </summary>
        /// <returns>
        /// The value.
        /// </returns>
        public AnalogValue Read()
        {
            return this.connection.Read(this.channel);
        }
    }
}