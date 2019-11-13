// <copyright file="PinDetectedEdges.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;

    /// <summary>
    /// Represents detected edges.
    /// </summary>
    [Flags]
    public enum PinDetectedEdges
    {
        /// <summary>
        /// No changes are detected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Rising edge changes are detected.
        /// </summary>
        Rising = 1,

        /// <summary>
        /// Falling edge changes are detected.
        /// </summary>
        Falling = 2,

        /// <summary>
        /// Both changes are detected.
        /// </summary>
        Both = Rising | Falling,
    }
}