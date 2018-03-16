// <copyright file="Processor.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi
{
    /// <summary>
    /// The Raspberry Pi processor.
    /// </summary>
    public enum Processor
    {
        /// <summary>
        /// Processor is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Processor is a BCM2708.
        /// </summary>
        Bcm2708,

        /// <summary>
        /// Processor is a BCM2709.
        /// </summary>
        Bcm2709,

        /// <summary>
        /// Processor is a BCM2835.
        /// </summary>
        Bcm2835,
    }
}