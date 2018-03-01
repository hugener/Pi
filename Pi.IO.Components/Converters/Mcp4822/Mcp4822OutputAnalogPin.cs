// <copyright file="Mcp4822OutputAnalogPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Converters.Mcp4822
{
    /// <summary>
    /// Represents an output pin for <see cref="Mcp4822SpiConnection"/>.
    /// </summary>
    /// <seealso cref="Pi.IO.IOutputAnalogPin" />
    public class Mcp4822OutputAnalogPin : IOutputAnalogPin
    {
        private readonly Mcp4822SpiConnection connection;
        private readonly Mcp4822Channel channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp4822OutputAnalogPin" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        public Mcp4822OutputAnalogPin(Mcp4822SpiConnection connection, Mcp4822Channel channel)
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
        /// Writes the specified value to the pin.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(AnalogValue value)
        {
            this.connection.Write(this.channel, value);
        }
    }
}