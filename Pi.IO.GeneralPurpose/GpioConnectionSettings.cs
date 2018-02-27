using System;
using System.Configuration;
using Pi.IO.GeneralPurpose.Configuration;

namespace Pi.IO.GeneralPurpose
{
    /// <summary>
    /// Represents settings for <see cref="GpioConnection"/>.
    /// </summary>
    public class GpioConnectionSettings
    {
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
        /// Gets the default blink duration.
        /// </summary>
        public static readonly TimeSpan DefaultBlinkDuration = TimeSpan.FromMilliseconds(250);

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

        /// <summary>
        /// Gets the default poll interval.
        /// </summary>
        public static TimeSpan DefaultPollInterval
        {
            get
            {
                var configurationSection = ConfigurationManager.GetSection("gpioConnection") as GpioConnectionConfigurationSection;
                return TimeSpan.FromMilliseconds(configurationSection != null
                           ? (double)configurationSection.PollInterval
                           : (double)GpioConnectionConfigurationSection.DefaultPollInterval);

            }
        }

        /// <summary>
        /// Gets the board connector pinout.
        /// </summary>
        /// <value>
        /// The board connector pinout.
        /// </value>
        public static ConnectorPinout ConnectorPinout
        {
            get
            {
                var configurationSection = ConfigurationManager.GetSection("gpioConnection") as GpioConnectionConfigurationSection;
                if (configurationSection != null)
                {
                    switch (configurationSection.BoardConnectorRevision)
                    {
                        case 1:
                            return ConnectorPinout.Rev1;
                        case 2:
                            return ConnectorPinout.Rev2;
                        case 3:
                            return ConnectorPinout.Plus;
                    }
                }

                return Board.Current.ConnectorPinout;
            }
        }
    }
}