// <copyright file="PinStatusEventArgs.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;

    /// <summary>
    /// Represents event arguments related to pin status.
    /// </summary>
    public class PinStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public PinConfiguration Configuration { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PinStatusEventArgs"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; internal set; }
    }
}