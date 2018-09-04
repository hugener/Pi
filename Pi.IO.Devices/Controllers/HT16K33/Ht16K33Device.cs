// <copyright file="Ht16K33Device.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.HT16K33
{
    using global::System;
    using Pi.IO.InterIntegratedCircuit;

    /// <summary>
    /// Driver for Holtek HT16K33 LED Matrix driver
    /// As used by Adafruit devices.
    /// </summary>
    public class Ht16K33Device // : IPwmDevice
    {
        /// <summary>
        /// The default address.
        /// </summary>
        public const byte DefaultAddress = 0x70;

        /// <summary>
        /// The HT16 K33 oscillator.
        /// </summary>
        public const byte Ht16K33Oscillator = 0x01;

        /// <summary>
        /// The HT16 K33 display on.
        /// </summary>
        public const byte Ht16K33DisplayOn = 0x01;

        private readonly I2cDeviceConnection connection;
        private readonly IHt16K33DeviceReporter ht16K33DeviceReporter;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Ht16K33Device" /> class.
        /// </summary>
        /// <param name="connection">I2c connection.</param>
        /// <param name="rowCount">Rows in use (1 to 16).</param>
        /// <param name="ht16K33DeviceReporter">The HT16 K33 device reporter.</param>
        public Ht16K33Device(I2cDeviceConnection connection, int rowCount, IHt16K33DeviceReporter ht16K33DeviceReporter = null)
        {
            this.LedBuffer = new byte[rowCount];
            this.connection = connection;
            this.ht16K33DeviceReporter = ht16K33DeviceReporter;

            this.ht16K33DeviceReporter?.Resetting();

            connection.Write((byte)Command.SystemSetup | (byte)Ht16K33Oscillator); //// Turn on the oscillator.
            connection.Write((byte)Command.Flash | (byte)Ht16K33DisplayOn | (byte)Flash.Off);
            connection.Write((byte)Command.DimmingSet | (byte)15);

            ////  connection.Write(SetupSequence);
        }

        /// <summary>
        /// The flash modes.
        /// </summary>
        public enum Flash : byte
        {
            /// <summary>
            /// The off
            /// </summary>
            Off = 0x00,

            /// <summary>
            /// The on
            /// </summary>
            On = 0x01,

            /// <summary>
            /// The two hz
            /// </summary>
            TwoHz = 0x02,

            /// <summary>
            /// The one hz
            /// </summary>
            OneHz = 0x04,

            /// <summary>
            /// The half hz
            /// </summary>
            HalfHz = 0x06,
        }

        /// <summary>
        /// The commands.
        /// </summary>
        public enum Command : byte
        {
            /// <summary>
            /// The display data address
            /// </summary>
            DisplayDataAddress = 0x00,

            /// <summary>
            /// The system setup
            /// </summary>
            SystemSetup = 0x20,

            /// <summary>
            /// The key data address pointer
            /// </summary>
            KeyDataAddressPointer = 0x40,

            /// <summary>
            /// The int flag address pointer
            /// </summary>
            IntFlagAddressPointer = 0x60,

            /// <summary>
            /// The flash
            /// </summary>
            Flash = 0x80,

            /// <summary>
            /// The row int set
            /// </summary>
            RowIntSet = 0xA0,

            /// <summary>
            /// The dimming set
            /// </summary>
            DimmingSet = 0xE0,

            /// <summary>
            /// The test mode
            /// </summary>
            TestMode = 0xD9,
        }

        /// <summary>
        /// Gets the led buffer.
        /// </summary>
        /// <value>
        /// The led buffer.
        /// </value>
        public byte[] LedBuffer { get; } // Max 16 rows, 8 bits (leds)

        /// <summary>
        /// Flash display at specified frequency.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        public void SetFlash(Flash frequency)
        {
            this.connection.WriteByte((byte)((byte)Command.Flash | Ht16K33DisplayOn | (byte)frequency));
        }

        /// <summary>
        /// Set brightness of entire display to specified value (0 to 15).
        /// </summary>
        /// <param name="brightness">The brightness.</param>
        public void SetBrightness(uint brightness)
        {
            if (brightness > 15)
            {
                brightness = 15;
            }

            this.connection.WriteByte((byte)((byte)Command.DimmingSet | (byte)brightness));
        }

        /// <summary>
        /// Sets specified LED (0-[row-count] rows, 0 to 7 leds).
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="led">The led.</param>
        /// <param name="outputOn">if set to <c>true</c> [output on].</param>
        /// <exception cref="Exception">
        /// Row out of range
        /// or
        /// LED out of range 0 to 7.
        /// </exception>
        public void SetLed(uint row, uint led, bool outputOn)
        {
            if (row >= this.LedBuffer.Length)
            {
                throw new Exception("Row out of range");
            }

            if (led > 7)
            {
                throw new Exception("LED out of range 0 to 7");
            }

            if (outputOn)
            {
                this.LedBuffer[row] |= (byte)(1 << (int)led); // Turn on the speciried LED (set bit to one).
            }
            else
            {
                this.LedBuffer[row] &= (byte)~(1 << (int)led);  // Turn off the specified LED (set bit to zero).
            }

            this.connection.Write((byte)row, this.LedBuffer[row]);
        }

        /// <summary>
        /// Write display buffer to display hardware.
        /// </summary>
        public void WriteDisplayBuffer()
        {
            for (int i = 0; i < this.LedBuffer.Length; i++)
            {
                this.connection.Write((byte)i, this.LedBuffer[i]);
            }
        }

        /// <summary>
        /// Clear contents of display buffer.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < this.LedBuffer.Length; i++)
            {
                this.LedBuffer[i] = 0;
            }

            this.WriteDisplayBuffer();
        }

        /// <summary>
        /// Set all LEDs On.
        /// </summary>
        public void SetAllOn()
        {
            for (int i = 0; i < this.LedBuffer.Length; i++)
            {
                this.LedBuffer[i] = 1;
            }

            this.WriteDisplayBuffer();
        }
    }
}
