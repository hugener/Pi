// <copyright file="Mcp23008I2cConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Expanders.Mcp23008
{
    using InterIntegratedCircuit;

    /// <summary>
    /// Represents a I2C connection to a MCP23008 I/O Expander.
    /// </summary>
    /// <remarks>See <see href="http://www.adafruit.com/datasheets/MCP23008.pdf"/> for more information.</remarks>
    public class Mcp23008I2CConnection
    {
        private readonly I2cDeviceConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp23008I2CConnection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Mcp23008I2CConnection(I2cDeviceConnection connection)
        {
            this.connection = connection;
        }

        private enum Register
        {
            Iodir = 0x00,
            Ipol = 0x01,
            Gpinten = 0x02,
            Defval = 0x03,
            Intcon = 0x04,
            Iocon = 0x05,
            Gppu = 0x06,
            Intf = 0x07,
            Intcap = 0x08,
            Gpio = 0x09,
            Olat = 0x0A
        }

        /// <summary>
        /// Sets the direction.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        public void SetDirection(Mcp23008Pin pin, Mcp23008PinDirection direction)
        {
            var register = Register.Iodir;

            this.connection.WriteByte((byte)register);
            var directions = this.connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newDirections = direction == Mcp23008PinDirection.Input
                                    ? directions | bit
                                    : directions & ~bit;

            this.connection.Write((byte)register, (byte)newDirections);
        }

        /// <summary>
        /// Sets the polarity.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="polarity">The polarity.</param>
        public void SetPolarity(Mcp23008Pin pin, Mcp23008PinPolarity polarity)
        {
            var register = Register.Ipol;

            this.connection.WriteByte((byte)register);
            var polarities = this.connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newPolarities = polarity == Mcp23008PinPolarity.Inverted
                                    ? polarities | bit
                                    : polarities & ~bit;

            this.connection.Write((byte)register, (byte)newPolarities);
        }

        /// <summary>
        /// Sets the resistor.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public void SetResistor(Mcp23008Pin pin, Mcp23008PinResistor resistor)
        {
            var register = Register.Gppu;

            this.connection.WriteByte((byte)register);
            var resistors = this.connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newResistors = resistor == Mcp23008PinResistor.PullUp
                                   ? resistors | bit
                                   : resistors & ~bit;

            this.connection.Write((byte)register, (byte)newResistors);
        }

        /// <summary>
        /// Sets the pin status.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="enabled">if set to <c>true</c>, pin is enabled.</param>
        public void SetPinStatus(Mcp23008Pin pin, bool enabled)
        {
            var register = Register.Gpio;

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
        public bool GetPinStatus(Mcp23008Pin pin)
        {
            var register = Register.Gpio;

            this.connection.WriteByte((byte)register);
            var status = this.connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            return (status & bit) != 0x00;
        }

        /// <summary>
        /// Toogles the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Toogle(Mcp23008Pin pin)
        {
            var register = Register.Gpio;

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
