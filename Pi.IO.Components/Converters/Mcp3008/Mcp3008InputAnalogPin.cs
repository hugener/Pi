// <copyright file="Mcp3008InputAnalogPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Converters.Mcp3008
{
    /// <summary>
    /// Analog input pin of the <see cref="Mcp3008SpiConnection"/>.
    /// </summary>
    /// <seealso cref="Pi.IO.IInputAnalogPin" />
    public class Mcp3008InputAnalogPin : IInputAnalogPin
    {
        private readonly Mcp3008SpiConnection connection;
        private readonly Mcp3008Channel channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp3008InputAnalogPin" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        public Mcp3008InputAnalogPin(Mcp3008SpiConnection connection, Mcp3008Channel channel)
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