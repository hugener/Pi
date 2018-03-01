// <copyright file="Pcf8574OutputBinaryPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Expanders.Pcf8574
{
    /// <summary>
    /// Represents a binary output pin on a MCP23017 I/O expander.
    /// </summary>
    public class Pcf8574OutputBinaryPin : IOutputBinaryPin
    {
        private readonly Pcf8574I2CConnection connection;
        private readonly Pcf8574Pin pin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pcf8574OutputBinaryPin"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        public Pcf8574OutputBinaryPin(Pcf8574I2CConnection connection, Pcf8574Pin pin)
        {
            this.connection = connection;
            this.pin = pin;

            connection.SetInputPin(pin, false);
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