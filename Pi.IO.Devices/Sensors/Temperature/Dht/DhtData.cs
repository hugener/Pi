// <copyright file="DhtData.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Sensors.Temperature.Dht
{
    /// <summary>
    /// Represents data from the Dht.
    /// </summary>
    public class DhtData
    {
        /// <summary>
        /// Gets or sets the attempt count.
        /// </summary>
        /// <value>
        /// The attempt count.
        /// </value>
        public int AttemptCount { get; set; }

        /// <summary>
        /// Gets or sets the temperature.
        /// </summary>
        /// <value>
        /// The temperature.
        /// </value>
        public UnitsNet.Temperature Temperature { get; set; }

        /// <summary>
        /// Gets or sets the relative humidity.
        /// </summary>
        /// <value>
        /// The relative humidity.
        /// </value>
        public UnitsNet.Ratio RelativeHumidity { get; set; }
    }
}