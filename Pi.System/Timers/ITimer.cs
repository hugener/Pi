// <copyright file="ITimer.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Timers
{
    using global::System;

    /// <summary>
    /// Provides an interface for a timer.
    /// </summary>
    public interface ITimer : IDisposable
    {
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        event EventHandler Tick;

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        TimeSpan Interval { get; set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="startDelay">The delay before the first occurence.</param>
        void Start(TimeSpan startDelay);

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
    }
}