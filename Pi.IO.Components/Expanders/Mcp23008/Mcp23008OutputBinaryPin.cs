// <copyright file="Mcp23008OutputBinaryPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Expanders.Mcp23008
{
    /// <summary>
    /// Represents a binary output pin on a MCP23008 I/O expander.
    /// </summary>
    public class Mcp23008OutputBinaryPin : IOutputBinaryPin
    {
        private readonly Mcp23008I2CConnection connection;
        private readonly Mcp23008Pin pin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp23008OutputBinaryPin"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        public Mcp23008OutputBinaryPin(
            Mcp23008I2CConnection connection,
            Mcp23008Pin pin,
            Mcp23008PinResistor resistor = Mcp23008PinResistor.None,
            Mcp23008PinPolarity polarity = Mcp23008PinPolarity.Normal)
        {
            this.connection = connection;
            this.pin = pin;

            connection.SetDirection(pin, Mcp23008PinDirection.Output);
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
