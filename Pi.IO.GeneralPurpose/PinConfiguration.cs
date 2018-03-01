// <copyright file="PinConfiguration.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;

    /// <summary>
    /// Represents the configuration of a pin.
    /// </summary>
    public abstract class PinConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PinConfiguration"/> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        protected PinConfiguration(ProcessorPin pin)
        {
            this.Pin = pin;
        }

        /// <summary>
        /// Gets the pin.
        /// </summary>
        public ProcessorPin Pin { get; }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public abstract PinDirection Direction { get; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PinConfiguration"/> is reversed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if reversed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>If set to <c>true</c>, pin value will be enabled when no signal is present, and disabled when a signal is present.</remarks>
        public bool Reversed { get; set; }

        /// <summary>
        /// Gets or sets the status changed action.
        /// </summary>
        /// <value>
        /// The status changed action.
        /// </value>
        public Action<bool> StatusChangedAction { get; set; }

        internal bool GetEffective(bool value)
        {
            return this.Reversed ? !value : value;
        }
    }
}