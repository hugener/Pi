// <copyright file="GpioConnectionDriverCapabilities.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;

    /// <summary>
    /// Represents capabilities of a driver.
    /// </summary>
    [Flags]
    public enum GpioConnectionDriverCapabilities
    {
        /// <summary>
        /// No advanced capability.
        /// </summary>
        None = 0,

        /// <summary>
        /// The driver can set pin resistor
        /// </summary>
        CanSetPinResistor = 1,

        /// <summary>
        /// The driver can set pin detected edges
        /// </summary>
        CanSetPinDetectedEdges = 2,

        /// <summary>
        /// The driver can change pin direction rapidly.
        /// </summary>
        CanChangePinDirectionRapidly = 4,

        /// <summary>
        /// The driver can work on third-party computers (not only Raspberry Pi)
        /// </summary>
        CanWorkOnThirdPartyComputers = 8,
    }
}