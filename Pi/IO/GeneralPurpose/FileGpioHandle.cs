// <copyright file="FileGpioHandle.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System.IO;

    /// <summary>
    /// Represents a handle on a GPIO file.
    /// </summary>
    public class FileGpioHandle
    {
        /// <summary>
        /// Gets or sets the gpio path.
        /// </summary>
        /// <value>
        /// The gpio path.
        /// </value>
        public string GpioPath { get; set; }

        /// <summary>
        /// Gets or sets the gpio stream.
        /// </summary>
        /// <value>
        /// The gpio stream.
        /// </value>
        public Stream GpioStream { get; set; }
    }
}