// <copyright file="TimerFactory.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Core.Timers
{
    using Sundew.Base.Disposal;
    using Sundew.Base.Timers;

    /// <summary>
    /// Factory for creating a timer.
    /// </summary>
    /// <seealso cref="ITimerFactory" />
    public class TimerFactory : ITimerFactory
    {
        private readonly DisposingList<ITimerControl> timers = new DisposingList<ITimerControl>();

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>A new timer.</returns>
        public ITimer Create()
        {
            return this.timers.Add(Timer.Create());
        }

        /// <summary>
        /// Creates the specified state.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The state.</param>
        /// <returns>A new timer.</returns>
        public ITimer<TState> Create<TState>(TState state)
        {
            return this.timers.Add(Timer.Create(state));
        }

        /// <summary>
        /// Disposes the specified timer control.
        /// </summary>
        /// <param name="timerControl">The timer control.</param>
        public void Dispose(ITimerControl timerControl)
        {
            this.timers.Dispose(timerControl);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.timers.Dispose();
        }
    }
}