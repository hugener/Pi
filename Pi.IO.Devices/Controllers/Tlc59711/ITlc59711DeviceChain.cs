// <copyright file="ITlc59711DeviceChain.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.Tlc59711
{
    using global::System;

    /// <summary>
    /// A connection the one or more TLC59711 devices.
    /// </summary>
    public interface ITlc59711DeviceChain : IDisposable
    {
        /// <summary>
        /// Gets the chained cluster of Adafruit's 12-channel 16bit PWM/LED driver TLC59711.
        /// </summary>
        ITlc59711Cluster Devices { get; }

        /// <summary>
        /// Creates a TLC59711 command and sends it to the first device using the SPI bus.
        /// </summary>
        void Update();
    }
}