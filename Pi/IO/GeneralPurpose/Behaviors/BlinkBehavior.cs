// <copyright file="BlinkBehavior.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose.Behaviors
{
    using global::System.Collections.Generic;
    using Pi.Core.Threading;

    /// <summary>
    /// Represents a simple blink behavior.
    /// </summary>
    public class BlinkBehavior : PinsBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkBehavior" /> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        /// <param name="threadFactory">The thread factory.</param>
        public BlinkBehavior(IEnumerable<PinConfiguration> configurations, IThreadFactory threadFactory = null)
            : base(configurations, ThreadFactory.EnsureThreadFactory(threadFactory))
        {
        }

        /// <summary>
        /// Gets or sets the number of times the behavior may blink.
        /// </summary>
        /// <value>
        /// The number of times the behavior may blink.
        /// </value>
        public int Count { get; set; }

        /// <summary>
        /// Gets the first step.
        /// </summary>
        /// <returns>
        /// The first step.
        /// </returns>
        protected override int GetFirstStep()
        {
            return 1;
        }

        /// <summary>
        /// Processes the step.
        /// </summary>
        /// <param name="step">The step.</param>
        protected override void ProcessStep(int step)
        {
            foreach (var configuration in this.Configurations)
            {
                this.Connection.Toggle(configuration);
            }
        }

        /// <summary>
        /// Tries to get the next step.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        ///   <c>true</c> if the behavior may continue; otherwise behavior will be stopped.
        /// </returns>
        protected override bool TryGetNextStep(ref int step)
        {
            step++;
            return step <= this.Count * 2;
        }
    }
}