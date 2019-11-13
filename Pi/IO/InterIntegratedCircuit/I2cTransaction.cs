// <copyright file="I2cTransaction.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.InterIntegratedCircuit
{
    using global::System;

    /// <summary>
    /// Defines an I2C data transaction.
    /// </summary>
    public class I2CTransaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2CTransaction"/> class.
        /// </summary>
        /// <param name="actions">The actions which should be performed within the transaction.</param>
        public I2CTransaction(params I2CAction[] actions)
        {
            this.Actions = actions ?? throw new ArgumentNullException("actions");
        }

        /// <summary>
        /// Gets the actions.
        /// </summary>
        public I2CAction[] Actions { get; }
    }
}
