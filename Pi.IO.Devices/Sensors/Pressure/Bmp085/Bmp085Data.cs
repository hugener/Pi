// <copyright file="Bmp085Data.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Sensors.Pressure.Bmp085
{
    /// <summary>
    /// Represents data from a <see cref="Bmp085Device"/>.
    /// </summary>
    public struct Bmp085Data
    {
        /// <summary>
        /// Gets or sets the temperature.
        /// </summary>
        /// <value>
        /// The temperature.
        /// </value>
        public UnitsNet.Temperature Temperature { get; set; }

        /// <summary>
        /// Gets or sets the pressure.
        /// </summary>
        /// <value>
        /// The pressure.
        /// </value>
        public UnitsNet.Pressure Pressure { get; set; }
    }
}