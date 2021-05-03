// <copyright file="Timer.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Core.Timers
{
    using Sundew.Base.Timers;

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
                       : new Sundew.Base.Timers.Timer();
        }

        /// <summary>
        /// Creates a timer.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The state.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The timer.
        /// </returns>
        /// <remarks>
        /// The created timer is the most suitable for the current platform.
        /// </remarks>
        public static ITimer<TState> Create<TState>(TState state, string name = null)
        {
            return Board.Current.IsRaspberryPi
                ? new HighResolutionTimer<TState>(state)
                : new Sundew.Base.Timers.Timer<TState>(state);
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