// <copyright file="IControlDevice.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Interop
{
    using System;

    /// <summary>
    /// A Linux I/O control device.
    /// </summary>
    public interface IControlDevice : IDisposable
    {
        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The data to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        int Control(uint request, ref uint data);

        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The data to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        int Control(uint request, ref byte data);

        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">An untyped pointer to memory that contains the command/request data.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        int Control(uint request, IntPtr data);
    }
}