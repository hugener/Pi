// <copyright file="InputPinChangedArgs.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Devices.PiFaceDigital
{
    using global::System;

    /// <summary>
    /// Event arg for input pin change.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class InputPinChangedArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputPinChangedArgs"/> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public InputPinChangedArgs(PiFaceInputPin pin)
        {
            this.Pin = pin;
        }

        /// <summary>
        /// Gets the pin.
        /// </summary>
        /// <value>
        /// The pin.
        /// </value>
        public PiFaceInputPin Pin { get; }
    }
}
