// <copyright file="I2cAction.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.InterIntegratedCircuit
{
    using global::System;

    /// <summary>
    /// Abstract i2c action.
    /// </summary>
    public abstract class I2CAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2CAction" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        protected I2CAction(byte[] buffer)
        {
            this.Buffer = buffer ?? throw new ArgumentNullException("buffer");
        }

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        /// <value>
        /// The buffer.
        /// </value>
        public byte[] Buffer { get; }
    }
}
