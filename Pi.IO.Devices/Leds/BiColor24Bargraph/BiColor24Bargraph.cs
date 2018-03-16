// <copyright file="BiColor24Bargraph.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Leds.BiColor24Bargraph
{
    using global::System;
    using Pi.IO.Devices.Controllers.HT16K33;
    using Pi.IO.InterIntegratedCircuit;

    /// <summary>
    /// Represents a BiColor24BarGraph.
    /// </summary>
    /// <seealso cref="Ht16K33Device" />
    public class BiColor24Bargraph : Ht16K33Device
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BiColor24Bargraph"/> class.
        /// </summary>
        /// <param name="connection">I2c Connection.</param>
        public BiColor24Bargraph(I2cDeviceConnection connection)
            : base(connection, 6)
        {
        }

        /// <summary>
        /// Defines the Led color state.
        /// </summary>
        public enum LedState
        {
            /// <summary>
            /// The off
            /// </summary>
            Off,

            /// <summary>
            /// The red
            /// </summary>
            Red,

            /// <summary>
            /// The green
            /// </summary>
            Green,

            /// <summary>
            /// The yellow
            /// </summary>
            Yellow,
        }

        /// <summary>
        /// Sets the led (0 to 23)
        /// </summary>
        /// <param name="ledNo">Led no.</param>
        /// <param name="state">State.</param>
        public void SetLed(uint ledNo, LedState state)
        {
            if (ledNo > 23)
            {
                throw new Exception("led must be between 0 and 23");
            }

            long r;
            r = Math.DivRem(ledNo, 4, out var c) * 2;
            if (ledNo >= 12)
            {
                c += 4;
            }

            if (r > 4)
            {
                r -= 6;
            }

            switch (state)
            {
                case LedState.Off:
                    this.SetLed((uint)r, (uint)c, false);
                    this.SetLed((uint)r + 1, (uint)c, false);
                    break;
                case LedState.Red:
                    this.SetLed((uint)r, (uint)c, true);
                    this.SetLed((uint)r + 1, (uint)c, false);
                    break;
                case LedState.Yellow:
                    this.SetLed((uint)r, (uint)c, true);
                    this.SetLed((uint)r + 1, (uint)c, true);
                    break;
                case LedState.Green:
                    this.SetLed((uint)r, (uint)c, false);
                    this.SetLed((uint)r + 1, (uint)c, true);
                    break;
            }
        }
    }
}
