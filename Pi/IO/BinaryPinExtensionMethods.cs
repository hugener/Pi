// <copyright file="BinaryPinExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO
{
    using System;

    /// <summary>
    /// Provides extension methods for binary pins.
    /// </summary>
    public static class BinaryPinExtensionMethods
    {
        /// <summary>
        /// Waits for a pin to reach the specified state, then measures the time it remains in this state.
        /// </summary>
        /// <param name="pin">The measure pin.</param>
        /// <param name="waitForUp">if set to <c>true</c>, wait for the pin to be up.</param>
        /// <param name="phase1Timeout">The first phase timeout.</param>
        /// <param name="phase2Timeout">The second phase timeout.</param>
        /// <returns>
        /// The time the pin remains up, in milliseconds.
        /// </returns>
        public static TimeSpan Time(this IInputBinaryPin pin, bool waitForUp = true, TimeSpan phase1Timeout = default(TimeSpan), TimeSpan phase2Timeout = default(TimeSpan))
        {
            pin.Wait(waitForUp, phase1Timeout);

            var waitDown = DateTime.UtcNow;
            pin.Wait(!waitForUp, phase2Timeout);

            return DateTime.UtcNow - waitDown;
        }
    }
}