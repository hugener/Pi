// <copyright file="GpioConnectionSettings.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;
    using Pi.Core;

    /// <summary>
    /// Represents settings for <see cref="GpioConnection"/>.
    /// </summary>
    public class GpioConnectionSettings
    {
        /// <summary>
        /// The default poll interval.
        /// </summary>
        public static readonly TimeSpan DefaultPollInterval = TimeSpan.FromMilliseconds(50);

        /// <summary>
        /// Gets the default blink duration.
        /// </summary>
        public static readonly TimeSpan DefaultBlinkDuration = TimeSpan.FromMilliseconds(250);

        private TimeSpan blinkDuration;
        private TimeSpan pollInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnectionSettings"/> class.
        /// </summary>
        public GpioConnectionSettings()
        {
            this.BlinkDuration = DefaultBlinkDuration;
            this.PollInterval = DefaultPollInterval;
            this.Opened = true;
        }

        /// <summary>
        /// Gets the board connector pinout.
        /// </summary>
        /// <value>
        /// The board connector pinout.
        /// </value>
        public static ConnectorPinout ConnectorPinout => Board.Current.ConnectorPinout;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GpioConnectionSettings"/> is opened on initialization.
        /// </summary>
        /// <value>
        ///   <c>true</c> if opened on initialization; otherwise, <c>false</c>.
        /// </value>
        public bool Opened { get; set; }

        /// <summary>
        /// Gets or sets the duration of the blink.
        /// </summary>
        /// <value>
        /// The duration of the blink, in milliseconds.
        /// </value>
        public TimeSpan BlinkDuration
        {
            get => this.blinkDuration;
            set => this.blinkDuration = value >= TimeSpan.Zero ? value : DefaultBlinkDuration;
        }

        /// <summary>
        /// Gets or sets the poll interval.
        /// </summary>
        /// <value>
        /// The poll interval.
        /// </value>
        public TimeSpan PollInterval
        {
            get => this.pollInterval;
            set => this.pollInterval = value >= TimeSpan.Zero ? value : DefaultPollInterval;
        }
    }
}