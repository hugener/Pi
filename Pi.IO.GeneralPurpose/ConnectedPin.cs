// <copyright file="ConnectedPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;
    using global::System.Collections.Generic;

    /// <summary>
    /// Represents a connected pin.
    /// </summary>
    public class ConnectedPin
    {
        private readonly GpioConnection connection;
        private readonly HashSet<EventHandler<PinStatusEventArgs>> events = new HashSet<EventHandler<PinStatusEventArgs>>();

        internal ConnectedPin(GpioConnection connection, PinConfiguration pinConfiguration)
        {
            this.connection = connection;
            this.Configuration = pinConfiguration;
        }

        /// <summary>
        /// Occurs when pin status changed.
        /// </summary>
        public event EventHandler<PinStatusEventArgs> StatusChanged
        {
            add
            {
                if (this.events.Count == 0)
                {
                    this.connection.PinStatusChanged += this.ConnectionPinStatusChanged;
                }

                this.events.Add(value);
            }

            remove
            {
                this.events.Remove(value);
                if (this.events.Count == 0)
                {
                    this.connection.PinStatusChanged -= this.ConnectionPinStatusChanged;
                }
            }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public PinConfiguration Configuration { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ConnectedPin"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get => this.connection[this.Configuration];
            set => this.connection[this.Configuration] = value;
        }

        /// <summary>
        /// Toggles this pin.
        /// </summary>
        public void Toggle()
        {
            this.connection.Toggle(this.Configuration);
        }

        /// <summary>
        /// Blinks the pin.
        /// </summary>
        /// <param name="duration">The blink duration.</param>
        public void Blink(TimeSpan duration = default(TimeSpan))
        {
            this.connection.Blink(this.Configuration, duration);
        }

        private void ConnectionPinStatusChanged(object sender, PinStatusEventArgs pinStatusEventArgs)
        {
            if (pinStatusEventArgs.Configuration.Pin != this.Configuration.Pin)
            {
                return;
            }

            foreach (var eventHandler in this.events)
            {
                eventHandler(sender, pinStatusEventArgs);
            }
        }
    }
}