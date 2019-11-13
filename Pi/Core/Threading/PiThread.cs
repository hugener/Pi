// <copyright file="PiThread.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Core.Threading
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Pi.Core.Timers;
    using Sundew.Base.Threading;

    internal class PiThread : ICurrentThread
    {
        private static readonly TimeSpan MinLongDelay = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan MinNanoDelay = TimeSpan.FromTicks(4500);
        private static readonly long NanoSleepOffset = Calibrate();
        private static readonly CurrentThread CurrentThread = new CurrentThread();

        /// <summary>
        /// Gets the managed thread identifier.
        /// </summary>
        /// <value>
        /// The managed thread identifier.
        /// </value>
        public int ManagedThreadId => Environment.CurrentManagedThreadId;

        /// <summary>
        /// Sleeps the specified delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public void Sleep(TimeSpan delay)
        {
            Sleep(delay, CurrentThread, CancellationToken.None);
        }

        /// <summary>
        /// Sleeps the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        public void Sleep(int milliseconds)
        {
            this.Sleep(TimeSpan.FromMilliseconds(milliseconds));
        }

        /// <summary>
        /// Delays the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>An async task.</returns>
        public Task Delay(TimeSpan timeSpan)
        {
            return CurrentThread.Delay(timeSpan);
        }

        /// <summary>
        /// Delays the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns>An async task.</returns>
        public Task Delay(int milliseconds)
        {
            return CurrentThread.Delay(milliseconds);
        }

        /// <summary>
        /// Sleeps the specified delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void Sleep(TimeSpan delay, CancellationToken cancellationToken)
        {
            Sleep(delay, CurrentThread, cancellationToken);
        }

        /// <summary>
        /// Sleeps the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void Sleep(int milliseconds, CancellationToken cancellationToken)
        {
            Sleep(TimeSpan.FromMilliseconds(milliseconds), CurrentThread, cancellationToken);
        }

        /// <summary>
        /// Delays the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An async task.</returns>
        public Task Delay(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            return CurrentThread.Delay(timeSpan, cancellationToken);
        }

        /// <summary>
        /// Delays the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An async task.</returns>
        public Task Delay(int milliseconds, CancellationToken cancellationToken)
        {
            return CurrentThread.Delay(milliseconds, cancellationToken);
        }

        /// <summary>
        /// Sleeps the specified delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="currentThread">The current thread.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   <c>true</c>, if the thread slept for the specified delay otherwise <c>false</c>.
        /// </returns>
        internal static bool Sleep(TimeSpan delay, ICurrentThread currentThread, CancellationToken cancellationToken)
        {
            // Based on [BCM2835 C library](http://www.open.com.au/mikem/bcm2835/)

            // Calling nanosleep() takes at least 100-200 us, so use it for
            // long waits and use a busy wait on the hires timer for the rest.
            var stopwatch = Stopwatch.StartNew();
            if (delay >= MinLongDelay || delay == Timeout.InfiniteTimeSpan)
            {
                // Do not use high resolution timer for long interval (>= 100ms)
                currentThread.Sleep(delay, cancellationToken);
                return !cancellationToken.IsCancellationRequested;
            }

            if (delay > MinNanoDelay)
            {
                var t1 = default(Interop.Timespec);
                var t2 = default(Interop.Timespec);

                // Use nanosleep if interval is higher than 450µs
                t1.TvSec = IntPtr.Zero;
                t1.TvNsec = (IntPtr)((delay.Ticks * 100) - NanoSleepOffset);

                Interop.Nanosleep(ref t1, ref t2);
                return true;
            }

            while (stopwatch.Elapsed < delay)
            {
            }

            return true;
        }

        private static long Calibrate()
        {
            const int referenceCount = 1000;
            const long calibrationDelayNanoSeconds = 1000000;
            var stopwatch = new Stopwatch();
            var ticksPerNanoSecond = Stopwatch.Frequency / 1000_000_000;
            return Enumerable.Range(0, referenceCount)
                .Aggregate(
                    0L,
                    (a, i) =>
                    {
                        var t1 = default(Interop.Timespec);
                        var t2 = default(Interop.Timespec);

                        t1.TvSec = IntPtr.Zero;
                        t1.TvNsec = (IntPtr)calibrationDelayNanoSeconds;

                        stopwatch.Restart();
                        Interop.Nanosleep(ref t1, ref t2);
                        stopwatch.Stop();
                        return a + ((stopwatch.ElapsedTicks * ticksPerNanoSecond) - calibrationDelayNanoSeconds);
                    },
                    a => a / referenceCount);
        }
    }
}