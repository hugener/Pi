// <copyright file="Mcp23017I2cConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Expanders.Mcp23017
{
    using InterIntegratedCircuit;

    /// <summary>
    /// Represents a I2C connection to a MCP23017 I/O Expander.
    /// </summary>
    /// <remarks>See <see href="http://www.adafruit.com/datasheets/mcp23017.pdf"/> for more information.</remarks>
    public class Mcp23017I2CConnection
    {
        private readonly I2cDeviceConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp23017I2CConnection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Mcp23017I2CConnection(I2cDeviceConnection connection)
        {
            this.connection = connection;
        }

        private enum Register
        {
            Iodira = 0x00,
            Iodirb = 0x01,
            Ipola = 0x02,
            Ipolb = 0x03,
            Gppua = 0x0c,
            Gppub = 0x0d,
            Gpioa = 0x12,
            Gpiob = 0x13
        }

        /// <summary>
        /// Sets the direction.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        public void SetDirection(Mcp23017Pin pin, Mcp23017PinDirection direction)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.Iodira : Register.Iodirb;

            this.connection.WriteByte((byte)register);
            var directions = this.connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newDirections = direction == Mcp23017PinDirection.Input
                                    ? directions | bit
                                    : directions & ~bit;

            this.connection.Write(new[] { (byte)register, (byte)newDirections });
        }

        /// <summary>
        /// Sets the polarity.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="polarity">The polarity.</param>
        public void SetPolarity(Mcp23017Pin pin, Mcp23017PinPolarity polarity)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.Ipola : Register.Ipolb;

            this.connection.WriteByte((byte)register);
            var polarities = this.connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newPolarities = polarity == Mcp23017PinPolarity.Inverted
                                    ? polarities | bit
                                    : polarities & ~bit;

            this.connection.Write(new[] { (byte)register, (byte)newPolarities });
        }

        /// <summary>
        /// Sets the resistor.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public void SetResistor(Mcp23017Pin pin, Mcp23017PinResistor resistor)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.Gppua : Register.Gppub;

            this.connection.WriteByte((byte)register);
            var resistors = this.connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newResistors = resistor == Mcp23017PinResistor.PullUp
                                   ? resistors | bit
                                   : resistors & ~bit;

            this.connection.Write(new[] { (byte)register, (byte)newResistors });
        }

        /// <summary>
        /// Sets the pin status.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="enabled">if set to <c>true</c>, pin is enabled.</param>
        public void SetPinStatus(Mcp23017Pin pin, bool enabled)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.Gpioa : Register.Gpiob;

            this.connection.WriteByte((byte)register);
            var status = this.connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newStatus = enabled
                                ? status | bit
                                : status & ~bit;

            this.connection.Write((byte)register, (byte)newStatus);
        }

        /// <summary>
        /// Gets the pin status.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin status.</returns>
        public bool GetPinStatus(Mcp23017Pin pin)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.Gpioa : Register.Gpiob;

            this.connection.WriteByte((byte)register);
            var status = this.connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            return (status & bit) != 0x00;
        }

        /// <summary>
        /// Toogles the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Toogle(Mcp23017Pin pin)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.Gpioa : Register.Gpiob;

            this.connection.WriteByte((byte)register);
            var status = this.connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var bitEnabled = (status & bit) != 0x00;
            var newBitEnabled = !bitEnabled;

            var newStatus = newBitEnabled
                                ? status | bit
                                : status & ~bit;

            this.connection.Write((byte)register, (byte)newStatus);
        }
    }
}