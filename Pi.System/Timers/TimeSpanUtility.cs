// <copyright file="TimeSpanUtility.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Timers
{
    using global::System;

    /// <summary>
    /// Provides utilities for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanUtility
    {
        /// <summary>
        /// Creates a timespan from a number of microseconds.
        /// </summary>
        /// <param name="microseconds">The microseconds.</param>
        /// <returns>A <see cref="TimeSpan"/> based on microseconds.</returns>
        public static TimeSpan FromMicroseconds(double microseconds)
        {
            return TimeSpan.FromTicks((long)(microseconds * 10));
        }
    }
}