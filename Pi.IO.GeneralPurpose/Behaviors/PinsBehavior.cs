// <copyright file="PinsBehavior.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose.Behaviors
{
    using System.Threading;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using Timers;

    /// <summary>
    /// Represents the pins behavior base class.
    /// </summary>
    public abstract class PinsBehavior : IDisposable
    {
        private readonly ITimer timer;
        private readonly IThread thread;
        private int currentStep;

        /// <summary>
        /// Initializes a new instance of the <see cref="PinsBehavior" /> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        /// <param name="threadFactory">The thread factory.</param>
        protected PinsBehavior(IEnumerable<PinConfiguration> configurations, IThreadFactory threadFactory)
        {
            this.Configurations = configurations.ToArray();
            this.thread = threadFactory.Create();

            this.timer = Timer.Create();
            this.timer.Interval = TimeSpan.FromMilliseconds(250);
            this.timer.Tick += this.OnTimer;
        }

        /// <summary>
        /// Gets the configurations.
        /// </summary>
        public PinConfiguration[] Configurations { get; }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public TimeSpan Interval
        {
            get => this.timer.Interval;
            set => this.timer.Interval = value;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        protected GpioConnection Connection { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            Timer.Dispose(this.timer);
            this.thread.Dispose();
        }

        internal void Start(GpioConnection connection)
        {
            this.Connection = connection;
            foreach (var pinConfiguration in this.Configurations)
            {
                connection[pinConfiguration] = false;
            }

            this.currentStep = this.GetFirstStep();
            this.timer.Start(TimeSpan.Zero);
        }

        internal void Stop()
        {
            this.timer.Stop();

            foreach (var pinConfiguration in this.Configurations)
            {
                this.Connection[pinConfiguration] = false;
            }
        }

        /// <summary>
        /// Gets the first step.
        /// </summary>
        /// <returns>The first step.</returns>
        protected abstract int GetFirstStep();

        /// <summary>
        /// Processes the step.
        /// </summary>
        /// <param name="step">The step.</param>
        protected abstract void ProcessStep(int step);

        /// <summary>
        /// Tries to get the next step.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns><c>true</c> if the behavior may continue; otherwise behavior will be stopped.</returns>
        protected abstract bool TryGetNextStep(ref int step);

        private void OnTimer(object sender, EventArgs e)
        {
            this.ProcessStep(this.currentStep);
            if (!this.TryGetNextStep(ref this.currentStep))
            {
                this.thread.Sleep(this.Interval);
                this.Stop();
            }
        }
    }
}