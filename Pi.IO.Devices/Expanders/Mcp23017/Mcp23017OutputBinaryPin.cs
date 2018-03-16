// <copyright file="Mcp23017OutputBinaryPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Expanders.Mcp23017
{
    /// <summary>
    /// Represents a binary output pin on a MCP23017 I/O expander.
    /// </summary>
    public class Mcp23017OutputBinaryPin : IOutputBinaryPin
    {
        private readonly Mcp23017Device connection;
        private readonly Mcp23017Pin pin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp23017OutputBinaryPin"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        public Mcp23017OutputBinaryPin(
            Mcp23017Device connection,
            Mcp23017Pin pin,
            Mcp23017PinResistor resistor = Mcp23017PinResistor.None,
            Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            this.connection = connection;
            this.pin = pin;

            connection.SetDirection(pin, Mcp23017PinDirection.Output);
            connection.SetResistor(pin, resistor);
            connection.SetPolarity(pin, polarity);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Writes the value of the pin.
        /// </summary>
        /// <param name="state">if set to <c>true</c>, pin is set to high state.</param>
        public void Write(bool state)
        {
            this.connection.SetPinStatus(this.pin, state);
        }
    }
}