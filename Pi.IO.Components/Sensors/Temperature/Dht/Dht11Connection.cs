// <copyright file="Dht11Connection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Sensors.Temperature.Dht
{
    using global::System;
    using UnitsNet;

    /// <summary>
    /// Represents a connection to a DHT11 sensor.
    /// </summary>
    public class Dht11Connection : DhtConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dht11Connection"/> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="autoStart">if set to <c>true</c> [automatic start].</param>
        public Dht11Connection(IInputOutputBinaryPin pin, bool autoStart = true)
            : base(pin, autoStart)
        {
        }

        /// <summary>
        /// Gets the default sampling interval.
        /// </summary>
        /// <value>
        /// The default sampling interval.
        /// </value>
        protected override TimeSpan DefaultSamplingInterval => TimeSpan.FromSeconds(1);

        /// <summary>
        /// Gets the wakeup interval.
        /// </summary>
        /// <value>
        /// The wakeup interval.
        /// </value>
        protected override TimeSpan WakeupInterval => TimeSpan.FromMilliseconds(18);

        /// <summary>
        /// Gets the DHT data.
        /// </summary>
        /// <param name="temperatureValue">The temperature value.</param>
        /// <param name="humidityValue">The humidity value.</param>
        /// <returns>The DhtData.</returns>
        protected override DhtData GetDhtData(int temperatureValue, int humidityValue)
        {
            return new DhtData
            {
                RelativeHumidity = Ratio.FromPercent(humidityValue / 256d),
                Temperature = Temperature.FromDegreesCelsius(temperatureValue / 256d)
            };
        }
    }
}