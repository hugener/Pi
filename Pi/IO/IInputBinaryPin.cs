// <copyright file="IInputBinaryPin.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO
{
    using global::System;

    /// <summary>
    /// Provides an interface for input, binary pins.
    /// </summary>
    public interface IInputBinaryPin : IDisposable
    {
        /// <summary>
        /// Reads the state of the pin.
        /// </summary>
        /// <returns><c>true</c> if the pin is in high state; otherwise, <c>false</c>.</returns>
        bool Read();

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up. Default value is <c>true</c>.</param>
        /// <param name="timeout">The timeout. Default value is <see cref="TimeSpan.Zero"/>.</param>
        /// <remarks>If <c>timeout</c> is set to <see cref="TimeSpan.Zero"/>, a default timeout is used instead.</remarks>
        void Wait(bool waitForUp = true, TimeSpan timeout = default(TimeSpan));
    }
}