// <copyright file="Hd44780DataPins.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Displays.Hd44780
{
    using GeneralPurpose;

    /// <summary>
    /// Represents the data pin for the Hd44780.
    /// </summary>
    public class Hd44780DataPins
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780DataPins"/> class.
        /// </summary>
        /// <param name="connectorPins">The connector pins.</param>
        public Hd44780DataPins(params ConnectorPin[] connectorPins)
        {
            this.ConnectorPins = connectorPins;
        }

        /// <summary>
        /// Gets the connector pins.
        /// </summary>
        /// <value>
        /// The connector pins.
        /// </value>
        public ConnectorPin[] ConnectorPins { get; }
    }
}