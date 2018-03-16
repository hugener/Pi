// <copyright file="Tmp36Device.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Sensors.Temperature.Tmp36
{
    using global::System;
    using UnitsNet;

    /// <summary>
    /// Represents a connection to a TMP36 temperature sensor.
    /// </summary>
    /// <remarks>See <see href="http://learn.adafruit.com/send-raspberry-pi-data-to-cosm"/>.</remarks>
    public class Tmp36Device : IDisposable
    {
        private readonly IInputAnalogPin inputPin;
        private readonly ElectricPotential referenceVoltage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tmp36Device"/> class.
        /// </summary>
        /// <param name="inputPin">The input pin.</param>
        /// <param name="referenceVoltage">The reference voltage.</param>
        public Tmp36Device(IInputAnalogPin inputPin, ElectricPotential referenceVoltage)
        {
            this.inputPin = inputPin;
            this.referenceVoltage = referenceVoltage;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Gets the temperature.
        /// </summary>
        /// <returns>The temperature.</returns>
        public UnitsNet.Temperature GetTemperature()
        {
            var voltage = this.referenceVoltage * (double)this.inputPin.Read().Relative;
            return UnitsNet.Temperature.FromDegreesCelsius((voltage.Volts * 100) - 50);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this.inputPin.Dispose();
        }
    }
}
