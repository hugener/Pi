// <copyright file="TimerFactory.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Timers
{
    /// <summary>
    /// Factory for creating a timer.
    /// </summary>
    /// <seealso cref="Pi.Timers.ITimerFactory" />
    public class TimerFactory : ITimerFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:Pi.Timers.ITimer" />.
        /// </returns>
        public ITimer Create()
        {
            return Timer.Create();
        }
    }
}