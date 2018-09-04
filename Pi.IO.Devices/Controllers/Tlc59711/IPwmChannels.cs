// <copyright file="IPwmChannels.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Controllers.Tlc59711
{
    /// <summary>
    /// The Pulse-width modulation (PWM) channels.
    /// </summary>
    public interface IPwmChannels
    {
        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Indexer, which will allow client code to use [] notation on the class instance itself to modify PWM channel values.
        /// </summary>
        /// <param name="index">channel index.</param>
        /// <returns>The current PWM value from <paramref name="index"/>.</returns>
        ushort this[int index] { get; set; }

        /// <summary>
        /// Returns the PWM value at the specified channel <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Channel index.</param>
        /// <returns>The PWM value at the specified channel <paramref name="index"/>.</returns>
        ushort Get(int index);

        /// <summary>
        /// Sets the PWM value at channel <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Channel index.</param>
        /// <param name="value">The PWM value.</param>
        void Set(int index, ushort value);
    }
}