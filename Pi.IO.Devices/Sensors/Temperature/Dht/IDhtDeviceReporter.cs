// <copyright file="IDhtDeviceReporter.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Sensors.Temperature.Dht
{
    using global::System;

    /// <summary>
    /// Reports information for <see cref="DhtDevice"/>.
    /// </summary>
    public interface IDhtDeviceReporter
    {
        /// <summary>
        /// Fails to read data.
        /// </summary>
        /// <param name="tryCount">The try count.</param>
        /// <param name="exception">The exception.</param>
        void FailToReadData(int tryCount, Exception exception);
    }
}