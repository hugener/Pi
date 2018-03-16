// <copyright file="ITlc59711Cluster.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.Tlc59711
{
    using global::System.Collections.Generic;

    /// <summary>
    /// A chained cluster of Adafruit's 12-channel 16bit PWM/LED driver TLC59711.
    /// The devices should be connected together with their SDTI/SDTO pins.
    /// </summary>
    public interface ITlc59711Cluster : IEnumerable<ITlc59711Device>, IPwmDevice
    {
        /// <summary>
        /// Gets the number of TLC59711 devices chained together
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns the TLC59711 device at the requested position
        /// </summary>
        /// <param name="index">TLC59711 index</param>
        /// <returns>TLC59711 device</returns>
        ITlc59711Device this[int index] { get; }

        /// <summary>
        /// Returns the TLC59711 device at the requested position
        /// </summary>
        /// <param name="index">TLC59711 index</param>
        /// <returns>TLC59711 device</returns>
        ITlc59711Device Get(int index);

        /// <summary>
        /// Set BLANK on/off at all connected devices.
        /// </summary>
        /// <param name="blank">If set to <c>true</c> all outputs are forced off.</param>
        void Blank(bool blank);
    }
}