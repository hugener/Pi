// <copyright file="PiFaceOutputPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Devices.PiFaceDigital
{
    /// <summary>
    /// Derivative of PiFacePin that allows setting of the internal state
    /// </summary>
    public class PiFaceOutputPin : PiFacePin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PiFaceOutputPin"/> class.
        /// </summary>
        /// <param name="pinNumber">Number of the pin in the range 0 to 7</param>
        internal PiFaceOutputPin(int pinNumber)
            : base(pinNumber)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pin is set in software but does not update the PiFaceDigital device
        /// Allows individual pins to be modified then everything updated with a call to UpdatePiFaceOutputPins
        /// </summary>
        public new bool State
        {
            get => base.State;
            set => base.State = value;
        }
    }
}
