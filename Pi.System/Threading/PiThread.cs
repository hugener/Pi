// <copyright file="PiThread.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.System.Threading
{
    using global::System;
    using global::System.Diagnostics;
    using global::System.Linq;
    using global::System.Threading;
    using Timers;

    internal class PiThread : IDisposableThread
    {
        private static readonly TimeSpan MinLongDelay = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan MinNanoDelay = TimeSpan.FromTicks(4500);
        private static readonly long NanoSleepOffset = Calibrate();
        private readonly AutoResetEvent sleepAutoResetEvent = new AutoResetEvent(false);
        private readonly ThreadFactory threadFactory;

        public PiThread(ThreadFactory threadFactory)
        {
            this.threadFactory = threadFactory;
        }

        public void Sleep(TimeSpan delay)
        {
            Sleep(delay, this.sleepAutoResetEvent);
        }

        public void Dispose()
        {
            this.threadFactory.Dispose(this);
        }

        void IDisposableThread.DisposeThread()
        {
            this.sleepAutoResetEvent.Dispose();
        }

        /// <summary>
        /// Sleeps the specified delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="sleepAutoResetEvent">The sleep automatic reset event.</param>
        /// <returns><c>true</c>, if the thread slept for the specified delay otherwise <c>false</c>.</returns>
        internal static bool Sleep(TimeSpan delay, AutoResetEvent sleepAutoResetEvent)
        {
            // Based on [BCM2835 C library](http://www.open.com.au/mikem/bcm2835/)

            // Calling nanosleep() takes at least 100-200 us, so use it for
            // long waits and use a busy wait on the hires timer for the rest.
            var stopwatch = Stopwatch.StartNew();
            if (delay >= MinLongDelay || delay == Timeout.InfiniteTimeSpan)
            {
                // Do not use high resolution timer for long interval (>= 100ms)
                return !sleepAutoResetEvent.WaitOne(delay);
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