// <copyright file="PiFacePin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Devices.PiFaceDigital
{
    using global::System;
    using global::System.Collections.Generic;

    /// <summary>
    /// Represents the pins on a PiFace Digital board
    /// </summary>
    public abstract class PiFacePin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PiFacePin"/> class.
        /// </summary>
        /// <param name="pinNumber">Number of the pin in the range 0 to 7</param>
        internal PiFacePin(int pinNumber)
        {
            if (pinNumber < 0 || pinNumber > 7)
            {
                throw new ArgumentOutOfRangeException("pin numbers must be in the range 0 to 7");
            }

            this.Mask = (byte)(1 << pinNumber);
            this.Id = pinNumber;
        }

        /// <summary>
        /// Gets or sets the bit mask that allows this pin to get / set it's value from a byte.
        /// </summary>
        public byte Mask { get; protected set; }

        /// <summary>
        /// Gets the id number of the pin 0..7.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the pin is set.
        /// </summary>
        public virtual bool State { get; protected set; }

        /// <summary>
        /// Returns a byte representing the state of all pins
        /// </summary>
        /// <param name="pins">collection of pins to aggregate over</param>
        /// <returns>byte of all pin state</returns>
        internal static byte AllPinState(IEnumerable<PiFacePin> pins)
        {
            byte allPinState = 0;
            foreach (var pin in pins)
            {
                if (pin.State)
                {
                    allPinState = (byte)(allPinState | pin.Mask);
                }
            }

            return allPinState;
        }

        /// <summary>
        /// Update this pin based on a byte that contains the data for every pin
        /// </summary>
        /// <param name="allPinState">byte with all pin values</param>
        internal virtual void Update(byte allPinState)
        {
            this.State = (allPinState & this.Mask) > 0;
        }
    }
}
