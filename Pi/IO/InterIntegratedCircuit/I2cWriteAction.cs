// <copyright file="I2cWriteAction.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.InterIntegratedCircuit
{
    /// <summary>
    /// Defines an I2C write action.
    /// </summary>
    public class I2CWriteAction : I2CAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2CWriteAction"/> class.
        /// </summary>
        /// <param name="buffer">The buffer with data which should be written.</param>
        public I2CWriteAction(params byte[] buffer)
            : base(buffer)
        {
        }
    }
}
