// <copyright file="Hd44780LcdDeviceSettings.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Pi.Core.Timers;

namespace Pi.IO.Devices.Displays.Hd44780
{
    using global::System;
    using global::System.Text;

    /// <summary>
    /// Settings for the <see cref="Hd44780LcdDevice"/>.
    /// </summary>
    public class Hd44780LcdDeviceSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdDeviceSettings"/> class.
        /// </summary>
        public Hd44780LcdDeviceSettings()
        {
            this.ScreenWidth = 20;
            this.ScreenHeight = 2;
            this.PatternWidth = 5;
            this.PatternHeight = 8;

            this.Encoding = new Hd44780A00Encoding();
            this.SyncDelay = TimeSpanUtility.FromMicroseconds(1);

            // RightToLeft = false;
        }

        /// <summary>
        /// Gets or sets the synchronize delay.
        /// Might depend on the actual display.
        /// </summary>
        /// <value>
        /// The synchronize delay.
        /// </value>
        public TimeSpan SyncDelay { get; set; }

        /// <summary>
        /// Gets or sets the width of the screen.
        /// </summary>
        /// <value>
        /// The width of the screen.
        /// </value>
        public int ScreenWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the screen.
        /// </summary>
        /// <value>
        /// The height of the screen.
        /// </value>
        public int ScreenHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the pattern.
        /// </summary>
        /// <value>
        /// The width of the pattern.
        /// </value>
        public int PatternWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the pattern.
        /// </summary>
        /// <value>
        /// The height of the pattern.
        /// </value>
        public int PatternHeight { get; set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>
        /// The encoding.
        /// </value>
        public Encoding Encoding { get; set; }

        // public bool RightToLeft { get; set; }
    }
}