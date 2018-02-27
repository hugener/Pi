using System;
using System.Linq;
using Pi.IO.GeneralPurpose;

namespace Pi.IO.Components.Displays.Hd44780
{
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

        /// <summary>
        /// The register select (RS) pin.
        /// </summary>
        public IOutputBinaryPin RegisterSelect { get; }

        /// <summary>
        /// The clock (EN) pin.
        /// </summary>
        public IOutputBinaryPin Clock { get; }

        /// <summary>
        /// The backlight pin.
        /// </summary>
        public IOutputBinaryPin Backlight { get; }

        /// <summary>
        /// The read write (RW) pin.
        /// </summary>
        public IOutputBinaryPin ReadWrite { get; }

        /// <summary>
        /// The data pins.
        /// </summary>
        public IOutputBinaryPin[] Data { get; }
    }
}