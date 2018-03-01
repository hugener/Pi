// <copyright file="PinResistor.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    /// <summary>
    /// Represents the resistor enabled on an input.
    /// </summary>
    public enum PinResistor
    {
        /// <summary>
        /// No resistor is enabled on the input.
        /// </summary>
        None,

        /// <summary>
        /// A pull-down resistor is enabled.
        /// </summary>
        PullDown,

        /// <summary>
        /// A pull-up resistor is enabled.
        /// </summary>
        PullUp
    }
}