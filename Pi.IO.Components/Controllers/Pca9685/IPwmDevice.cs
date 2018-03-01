// <copyright file="IPwmDevice.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Controllers.Pca9685
{
    using global::System;
    using UnitsNet;

    /// <summary>
    /// Provides an interface for PWM devices.
    /// </summary>
    public interface IPwmDevice : IDisposable
    {
        /// <summary>
        /// Sets the PWM update rate.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        void SetPwmUpdateRate(Frequency frequency);

        /// <summary>
        /// Sets a single PWM channel with on / off values to control the duty cycle
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="on">The on values.</param>
        /// <param name="off">The off values.</param>
        void SetPwm(PwmChannel channel, int on, int off);

        /// <summary>
        /// Set a channel to fully on or off
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="fullOn">if set to <c>true</c>, all values are on; otherwise they are all off.</param>
        void SetFull(PwmChannel channel, bool fullOn);
    }
}