// <copyright file="InputPinConfiguration.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    /// <summary>
    /// Represents configuration of an input pin.
    /// </summary>
    public class InputPinConfiguration : PinConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputPinConfiguration"/> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public InputPinConfiguration(ProcessorPin pin)
            : base(pin)
        {
        }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public override PinDirection Direction => PinDirection.Input;

        /// <summary>
        /// Gets or sets the resistor.
        /// </summary>
        /// <value>
        /// The resistor.
        /// </value>
        public PinResistor Resistor { get; set; }
    }
}