// <copyright file="ITlc59711Device.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.Tlc59711
{
    /// <summary>
    /// Adafruit 12-Channel 16-bit PWM LED Driver TLC59711
    /// </summary>
    public interface ITlc59711Device : IPwmDevice, ITlc59711Settings
    {
        /// <summary>
        /// Initializes the device with default values.
        /// </summary>
        void Reset();
    }
}