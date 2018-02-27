// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PiThread.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Pi.Timers;

namespace Pi.System.Threading
{
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
                var t1 = new Interop.timespec();
                var t2 = new Interop.timespec();

                // Use nanosleep if interval is higher than 450µs
                t1.tv_sec = IntPtr.Zero;
                try
                {
                    t1.tv_nsec = (IntPtr)(delay.Ticks * 100 - NanoSleepOffset);
                }
                catch (Exception e)
                {
                    global::System.Console.WriteLine(e + " - " + delay);
                    throw;
                }

                Interop.nanosleep(ref t1, ref t2);
                return true;
            }

            while (stopwatch.Elapsed < delay);
            return true;
        }

        private static long Calibrate()
        {
            const int referenceCount = 1000;
            var stopwatch = new Stopwatch();
            var ticksPerNanoSecond = Stopwatch.Frequency / 1000_000_000;
            Console.WriteLine("Calibrating " + Stopwatch.Frequency);
            return Enumerable.Range(0, referenceCount)
                .Aggregate(
                    (long)0,
                    (a, i) =>
                    {
                        var t1 = new Interop.timespec();
                        var t2 = new Interop.timespec();

                        t1.tv_sec = IntPtr.Zero;
                        t1.tv_nsec = (IntPtr)1000000;

                        stopwatch.Restart();
                        Interop.nanosleep(ref t1, ref t2);
                        stopwatch.Stop();
                        return a + (stopwatch.ElapsedTicks * ticksPerNanoSecond - 1000000);
                    },
                    a => a / referenceCount);
        }
    }
}