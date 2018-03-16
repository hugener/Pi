// <copyright file="IPwmDevice.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.Tlc59711
{
    /// <summary>
    /// A pulse-width modulation (PWM) device
    /// </summary>
    public interface IPwmDevice
    {
        /// <summary>
        /// Gets the PWM channels
        /// </summary>
        IPwmChannels Channels { get; }
    }
}