// <copyright file="Hd44780LcdConnectionSettings.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Displays.Hd44780
{
    using global::System.Text;

    /// <summary>
    /// Settings for the <see cref="Hd44780LcdConnection"/>.
    /// </summary>
    public class Hd44780LcdConnectionSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdConnectionSettings"/> class.
        /// </summary>
        public Hd44780LcdConnectionSettings()
        {
            this.ScreenWidth = 20;
            this.ScreenHeight = 2;
            this.PatternWidth = 5;
            this.PatternHeight = 8;

            this.Encoding = new Hd44780A00Encoding();

            // RightToLeft = false;
        }

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