// <copyright file="GroveRgbDevice.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Leds.GroveRgb
{
    using global::Pi.System.Threading;
    using global::Pi.Timers;
    using global::System;
    using global::System.Collections.Generic;

    /// <summary>
    /// Represents a connection with Grove Chainable RGB Led modules.
    /// <see href="http://www.seeedstudio.com/wiki/Grove_-_Chainable_RGB_LED" />
    /// </summary>
    public class GroveRgbDevice : IDisposable
    {
        private static readonly TimeSpan Delay = TimeSpanUtility.FromMicroseconds(20);
        private readonly IThread thread;
        private readonly IOutputBinaryPin dataPin;
        private readonly IOutputBinaryPin clockPin;
        private readonly List<RgbColor> ledColors;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroveRgbDevice"/> class.
        /// </summary>
        /// <param name="dataPin">The data pin.</param>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="ledCount">The led count.</param>
        /// <param name="threadFactory">The thread factory.</param>
        public GroveRgbDevice(IOutputBinaryPin dataPin, IOutputBinaryPin clockPin, int ledCount, IThreadFactory threadFactory = null)
        {
            this.thread = ThreadFactory.EnsureThreadFactory(threadFactory).Create();
            this.ledColors = new List<RgbColor>();
            for (int i = 0; i < ledCount; i++)
            {
                // Initialize all leds with white color
                this.ledColors.Add(new RgbColor());
            }

            this.dataPin = dataPin;
            this.clockPin = clockPin;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Sets the color of a led.
        /// </summary>
        /// <param name="ledNumber">Led number (zero based index).</param>
        /// <param name="color">The color.</param>
        public void SetColor(int ledNumber, RgbColor color)
        {
            // Send data frame prefix (32x "0")
            this.SendByte(0x00);
            this.SendByte(0x00);
            this.SendByte(0x00);
            this.SendByte(0x00);

            // Send color data for each one of the leds
            for (int i = 0; i < this.ledColors.Count; i++)
            {
                if (i == ledNumber)
                {
                    this.ledColors[i].Red = color.Red;
                    this.ledColors[i].Green = color.Green;
                    this.ledColors[i].Blue = color.Blue;
                }

                // Start by sending a byte with the format "1 1 /B7 /B6 /G7 /G6 /R7 /R6"
                byte prefix = Convert.ToByte("11000000", 2);
                if ((color.Blue & 0x80) == 0)
                {
                    prefix |= Convert.ToByte("00100000", 2);
                }

                if ((color.Blue & 0x40) == 0)
                {
                    prefix |= Convert.ToByte("00010000", 2);
                }

                if ((color.Green & 0x80) == 0)
                {
                    prefix |= Convert.ToByte("00001000", 2);
                }

                if ((color.Green & 0x40) == 0)
                {
                    prefix |= Convert.ToByte("00000100", 2);
                }

                if ((color.Red & 0x80) == 0)
                {
                    prefix |= Convert.ToByte("00000010", 2);
                }

                if ((color.Red & 0x40) == 0)
                {
                    prefix |= Convert.ToByte("00000001", 2);
                }

                this.SendByte(prefix);

                // Now must send the 3 colors
                this.SendByte(this.ledColors[i].Blue);
                this.SendByte(this.ledColors[i].Green);
                this.SendByte(this.ledColors[i].Red);
            }

            // Terminate data frame (32x "0")
            this.SendByte(0x00);
            this.SendByte(0x00);
            this.SendByte(0x00);
            this.SendByte(0x00);
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            this.dataPin.Dispose();
            this.clockPin.Dispose();
            this.thread.Dispose();
        }

        private void SendByte(byte data)
        {
            // Send one bit at a time, starting with the MSB
            for (byte i = 0; i < 8; i++)
            {
                // If MSB is 1, write one and clock it, else write 0 and clock
                this.dataPin.Write((data & 0x80) != 0);

                // clk():
                this.clockPin.Write(false);
                this.thread.Sleep(Delay);
                this.clockPin.Write(true);
                this.thread.Sleep(Delay);

                // Advance to the next bit to send
                data <<= 1;
            }
        }
    }
}
