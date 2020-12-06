// <copyright file="SwitchInputPinConfiguration.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    /// <summary>
    /// Represents the configuration of an input pin acting as a switch.
    /// </summary>
    public class SwitchInputPinConfiguration : InputPinConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchInputPinConfiguration"/> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public SwitchInputPinConfiguration(ProcessorPin pin)
            : base(pin)
        {
        }

        internal SwitchInputPinConfiguration(InputPinConfiguration pin)
            : base(pin.Pin)
        {
            this.Reversed = pin.Reversed;
            this.Name = pin.Name;
        }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public override PinDirection Direction => PinDirection.Input;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SwitchInputPinConfiguration"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }
    }
}