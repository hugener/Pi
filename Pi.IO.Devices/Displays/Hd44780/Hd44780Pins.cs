// <copyright file="Hd44780Pins.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Displays.Hd44780
{
    using global::Pi.IO.GeneralPurpose;
    using global::System;
    using global::System.Linq;

    /// <summary>
    /// Represents the pins of a HD44780 LCD display.
    /// </summary>
    internal class Hd44780Pins : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780Pins" /> class.
        /// </summary>
        /// <param name="gpioConnectionDriver">The gpio connection driver.</param>
        /// <param name="registerSelect">The register select.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="backlight">The backlight.</param>
        /// <param name="readWrite">The read write.</param>
        /// <param name="data">The data.</param>
        public Hd44780Pins(
            IGpioConnectionDriver gpioConnectionDriver,
            ConnectorPin registerSelect,
            ConnectorPin clock,
            ConnectorPin? backlight,
            ConnectorPin? readWrite,
            params ConnectorPin[] data)
        {
            this.RegisterSelect = gpioConnectionDriver.Out(registerSelect);
            this.Clock = gpioConnectionDriver.Out(clock);
            if (backlight != null)
            {
                this.Backlight = gpioConnectionDriver.Out(backlight.Value);
            }

            if (readWrite != null)
            {
                this.ReadWrite = gpioConnectionDriver.Out(readWrite.Value);
            }

            this.Data = data.Select(gpioConnectionDriver.Out).ToArray();
        }

        /// <summary>
        /// Gets the register select (RS) pin.
        /// </summary>
        public IOutputBinaryPin RegisterSelect { get; }

        /// <summary>
        /// Gets the clock (EN) pin.
        /// </summary>
        public IOutputBinaryPin Clock { get; }

        /// <summary>
        /// Gets the backlight pin.
        /// </summary>
        public IOutputBinaryPin Backlight { get; }

        /// <summary>
        /// Gets the read write (RW) pin.
        /// </summary>
        public IOutputBinaryPin ReadWrite { get; }

        /// <summary>
        /// Gets the data pins.
        /// </summary>
        public IOutputBinaryPin[] Data { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.RegisterSelect.Dispose();
            this.Clock.Dispose();

            this.Backlight?.Dispose();
            this.ReadWrite?.Dispose();

            foreach (var dataPin in this.Data)
            {
                dataPin.Dispose();
            }
        }
    }
}