// <copyright file="I2cReadAction.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.InterIntegratedCircuit
{
    /// <summary>
    /// Defines an I2C read action.
    /// </summary>
    public class I2CReadAction : I2CAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2CReadAction"/> class.
        /// </summary>
        /// <param name="buffer">The buffer which should be used to store the received data.</param>
        public I2CReadAction(params byte[] buffer)
            : base(buffer)
        {
        }
    }
}
