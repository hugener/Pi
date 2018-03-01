// <copyright file="Bmp085I2cConnectionExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Sensors.Pressure.Bmp085
{
    using global::System;
    using UnitsNet;

    /// <summary>
    /// Provides extension methods for <see cref="Bmp085I2CConnection"/>.
    /// </summary>
    public static class Bmp085I2CConnectionExtensionMethods
    {
        /// <summary>
        /// Gets the sea-level pressure.
        /// </summary>
        /// <param name="connection">The BMP085 connection.</param>
        /// <param name="currentAltitude">The current altitude.</param>
        /// <returns>The pressure.</returns>
        public static Pressure GetSealevelPressure(this Bmp085I2CConnection connection, Length currentAltitude)
        {
            var pressure = connection.GetPressure();
            return Pressure.FromPascals(pressure.Pascals / Math.Pow(1.0 - (currentAltitude.Meters / 44330), 5.255));
        }

        /// <summary>
        /// Gets the altitude.
        /// </summary>
        /// <param name="connection">The BMP085 connection.</param>
        /// <param name="sealevelPressure">The sealevel pressure.</param>
        /// <returns>The altitude</returns>
        public static Length GetAltitude(this Bmp085I2CConnection connection, Pressure sealevelPressure)
        {
            var pressure = connection.GetPressure();
            return Length.FromMeters(44330 * (1.0 - Math.Pow(pressure.Pascals / sealevelPressure.Pascals, 1 / 5.255)));
        }
    }
}