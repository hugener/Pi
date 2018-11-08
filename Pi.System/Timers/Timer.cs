// <copyright file="Timer.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Timers
{
    using Sundew.Base.Threading;

    /// <summary>
    /// Provides access to timing features.
    /// </summary>
    public static class Timer
    {
        /// <summary>
        /// Creates a timer.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The timer.
        /// </returns>
        /// <remarks>
        /// The created timer is the most suitable for the current platform.
        /// </remarks>
        public static ITimer Create(string name = null)
        {
            return Board.Current.IsRaspberryPi
                       ? (ITimer)new HighResolutionTimer()
                       : new Sundew.Base.Threading.Timer();
        }

        /// <summary>
        /// Disposes the specified timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        public static void Dispose(ITimer timer)
        {
            timer.Dispose();
        }
    }
}