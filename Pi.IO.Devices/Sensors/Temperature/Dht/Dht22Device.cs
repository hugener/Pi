// <copyright file="Dht22Device.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Pi.Core.Threading;

namespace Pi.IO.Devices.Sensors.Temperature.Dht
{
    using global::System;
    using UnitsNet;

    /// <summary>
    /// Represents a connection to a DHT22 sensor.
    /// </summary>
    public class Dht22Device : DhtDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dht22Device" /> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="autoStart">if set to <c>true</c> [automatic start].</param>
        /// <param name="threadFactory">The thread factory.</param>
        /// <param name="dhtDeviceReporter">The DHT device reporter.</param>
        public Dht22Device(IInputOutputBinaryPin pin, bool autoStart = true, IThreadFactory threadFactory = null, IDhtDeviceReporter dhtDeviceReporter = null)
            : base(pin, autoStart, threadFactory, dhtDeviceReporter)
        {
        }

        /// <summary>
        /// Gets the default sampling interval.
        /// </summary>
        /// <value>
        /// The default sampling interval.
        /// </value>
        protected override TimeSpan DefaultSamplingInterval => TimeSpan.FromSeconds(2);

        /// <summary>
        /// Gets the wakeup interval.
        /// </summary>
        /// <value>
        /// The wakeup interval.
        /// </value>
        protected override TimeSpan WakeupInterval => TimeSpan.FromMilliseconds(1);

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
                RelativeHumidity = Ratio.FromPercent(humidityValue / 10d),
                Temperature = UnitsNet.Temperature.FromDegreesCelsius(temperatureValue / 10d),
            };
        }
    }
}