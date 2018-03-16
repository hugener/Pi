// <copyright file="StandardTimer.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Timers
{
    using global::System;

    /// <summary>
    /// Represents a timer.
    /// </summary>
    internal class StandardTimer : ITimer
    {
        private TimeSpan interval;
        private EventHandler tick;

        private bool isStarted;
        private global::System.Threading.Timer timer;

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public event EventHandler Tick
        {
            add
            {
                this.tick += value;
                this.StopIfHandlerEmpty();
            }

            remove
            {
                this.tick -= value;
                this.StopIfHandlerEmpty();
            }
        }

        /// <summary>
        /// Gets or sets the interval, in milliseconds.
        /// </summary>
        /// <value>
        /// The interval, in milliseconds.
        /// </value>
        public TimeSpan Interval
        {
            get => this.interval;
            set
            {
                this.interval = value;
                if (this.isStarted)
                {
                    this.Start(TimeSpan.Zero);
                }
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="startDelay">The delay before the first occurence, in milliseconds.</param>
        public void Start(TimeSpan startDelay)
        {
            lock (this)
            {
                if (!this.isStarted && this.interval.TotalMilliseconds >= 1)
                {
                    this.isStarted = true;
                    this.timer = new global::System.Threading.Timer(this.OnElapsed, null, startDelay, this.interval);
                }
                else
                {
                    this.Stop();
                }
            }
        }

        /// <summary>
        /// Restarts the specified start delay.
        /// </summary>
        /// <param name="startDelay">The start delay.</param>
        public void Restart(TimeSpan startDelay)
        {
            this.Stop();
            this.Start(startDelay);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                if (this.isStarted)
                {
                    this.isStarted = false;
                    this.timer.Dispose();
                    this.timer = null;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.tick = null;
        }

        private void OnElapsed(object state)
        {
            this.tick?.Invoke(this, EventArgs.Empty);
        }

        private void StopIfHandlerEmpty()
        {
            if (this.tick == null)
            {
                this.Stop();
            }
        }
    }
}