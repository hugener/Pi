// <copyright file="Hd44780Position.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Displays.Hd44780
{
    /// <summary>
    /// Represents the position of the cursor on a Hd44780 display.
    /// </summary>
    public struct Hd44780Position
    {
        /// <summary>
        /// The zero.
        /// </summary>
        public static Hd44780Position Zero = new Hd44780Position { Row = 0, Column = 0 };

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>
        /// The row.
        /// </value>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public int Column { get; set; }
    }
}