// <copyright file="GroveBarConnection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Leds.GroveBar
{
    using System.Threading;
    using global::System;
    using global::System.Text;

    /// <summary>
    /// Represents a connection with Grove Led Bar module.
    /// @see http://www.seeedstudio.com/wiki/Grove_-_LED_Bar
    /// </summary>
    public class GroveBarConnection : IDisposable
    {
        private const uint CommandMode = 0x0000;
        private static readonly TimeSpan Delay = TimeSpan.FromTicks(1);

        private readonly IOutputBinaryPin dataPin;
        private readonly IInputOutputBinaryPin clockPin;
        private readonly IThread thread;
        private string currentLedsStatus = "0000000000";

        /// <summary>
        /// Initializes a new instance of the <see cref="GroveBarConnection"/> class.
        /// </summary>
        /// <param name="dataPin">The data pin.</param>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="threadFactory">The thread factory.</param>
        public GroveBarConnection(IOutputBinaryPin dataPin, IInputOutputBinaryPin clockPin, IThreadFactory threadFactory = null)
        {
            this.dataPin = dataPin;
            this.clockPin = clockPin;
            this.thread = ThreadFactory.EnsureThreadFactory(threadFactory).Create();
            this.Initialize();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Sets status of leds from a binary string eg: "0010101011", where "0" is off and "1" is on
        /// </summary>
        /// <param name="ledsString">Leds string.</param>
        public void SetFromString(string ledsString)
        {
            this.currentLedsStatus = ledsString;
            this.SendData(CommandMode);
            var indexBits = (uint)Convert.ToInt32(ledsString, 2);
            for (int i = 0; i < 12; i++)
            {
                var state = (uint)((indexBits & 0x0001) > 0 ? 0x00FF : 0x0000);
                this.SendData(state);
                indexBits = indexBits >> 1;
            }

            this.LatchData();
        }

        /// <summary>
        /// Sets the level of the leds bar.
        /// </summary>
        /// <param name="level">Level.</param>
        public void SetLevel(int level)
        {
            var status = new StringBuilder(new string('0', 10));
            for (int i = 0; i < level; i++)
            {
                status[i] = '1';
            }

            this.currentLedsStatus = status.ToString();
            this.SendData(CommandMode);
            for (int i = 0; i < 12; i++)
            {
                var state = (uint)(i < level ? 0x00FF : 0x0000);
                this.SendData(state);
            }

            this.LatchData();
        }

        /// <summary>
        /// Turn on a single led at a given position (0-9)
        /// </summary>
        /// <param name="position">Position.</param>
        public void On(int position)
        {
            var status = new StringBuilder(this.currentLedsStatus);
            status[position] = '1';
            this.currentLedsStatus = status.ToString();
            this.SetFromString(this.currentLedsStatus);
        }

        /// <summary>
        /// Turn off a single led at a given position (0-9)
        /// </summary>
        /// <param name="position">Position.</param>
        public void Off(int position)
        {
            var status = new StringBuilder(this.currentLedsStatus);
            status[position] = '0';
            this.currentLedsStatus = status.ToString();
            this.SetFromString(this.currentLedsStatus);
        }

        /// <summary>
        /// Turn all leds on.
        /// </summary>
        public void AllOn()
        {
            this.currentLedsStatus = new string('1', 10);
            this.SetFromString(this.currentLedsStatus);
        }

        /// <summary>
        /// Turn all leds off.
        /// </summary>
        public void AllOff()
        {
            this.currentLedsStatus = new string('0', 10);
            this.SetFromString(this.currentLedsStatus);
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

        private void Initialize()
        {
            this.dataPin.Write(false);
            this.thread.Sleep(Delay);
            for (int i = 0; i < 4; i++)
            {
                this.dataPin.Write(true);
                this.dataPin.Write(false);
            }
        }

        private void SendData(uint data)
        {
            // Send 16 bit data
            for (int i = 0; i < 16; i++)
            {
                bool state = (data & 0x8000) > 0;
                this.dataPin.Write(state);
                state = !this.clockPin.Read();
                this.clockPin.Write(state);
                data <<= 1;
            }
        }

        private void LatchData()
        {
            this.dataPin.Write(false);
            this.thread.Sleep(Delay);
            for (int i = 0; i < 4; i++)
            {
                this.dataPin.Write(true);
                this.dataPin.Write(false);
            }
        }
    }
}
