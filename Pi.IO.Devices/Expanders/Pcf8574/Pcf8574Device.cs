// <copyright file="Pcf8574Device.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Expanders.Pcf8574
{
    using global::System;
    using global::System.Globalization;
    using Pi.IO.InterIntegratedCircuit;

    /// <summary>
    /// Represents a I2C connection to a PCF8574 I/O Expander.
    /// </summary>
    /// <remarks>See <see href="http://www.ti.com/lit/ds/symlink/pcf8574.pdf"/> for more information.</remarks>
    public class Pcf8574Device
    {
        private readonly I2cDeviceConnection connection;

        private byte inputPins;
        private byte currentStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pcf8574Device"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Pcf8574Device(I2cDeviceConnection connection)
        {
            this.connection = connection;
            connection.WriteByte(0);
        }

        /// <summary>
        /// Sets the pin status.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="enabled">if set to <c>true</c>, specified pin is enabled.</param>
        public void SetPinStatus(Pcf8574Pin pin, bool enabled)
        {
            var bit = GetPinBit(pin);
            if ((this.inputPins & bit) != 0x00)
            {
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Cannot set value of input pin {0}", pin));
            }

            var status = this.currentStatus;
            var newStatus = (byte)(enabled
                ? status | bit
                : status & ~bit);

            this.connection.Write((byte)(newStatus | this.inputPins));
            this.currentStatus = newStatus;
        }

        /// <summary>
        /// Gets the pin status.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin status.</returns>
        public bool GetPinStatus(Pcf8574Pin pin)
        {
            var bit = GetPinBit(pin);
            if ((this.inputPins & bit) == 0x00)
            {
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Cannot get value of input pin {0}", pin));
            }

            var status = this.connection.ReadByte();
            return (status & bit) != 0x00;
        }

        /// <summary>
        /// Toogles the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Toogle(Pcf8574Pin pin)
        {
            var bit = GetPinBit(pin);
            if ((this.inputPins & bit) != 0x00)
            {
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Cannot set value of input pin {0}", pin));
            }

            var status = this.currentStatus;

            var bitEnabled = (status & bit) != 0x00;
            var newBitEnabled = !bitEnabled;

            var newStatus = (byte)(newBitEnabled
                ? status | bit
                : status & ~bit);

            this.connection.Write((byte)(newStatus | this.inputPins));
            this.currentStatus = newStatus;
        }

        internal void SetInputPin(Pcf8574Pin pin, bool isInput)
        {
            var bit = GetPinBit(pin);
            this.inputPins = (byte)(isInput
                ? this.inputPins | bit
                : this.inputPins & ~bit);

            this.connection.Write((byte)(this.currentStatus | this.inputPins));
        }

        private static byte GetPinBit(Pcf8574Pin pin)
        {
            return (byte)(int)pin;
        }
    }
}