// <copyright file="ControlDevice.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Interop
{
    using global::System;
    using global::System.Runtime.InteropServices;

    /// <summary>
    /// A Linux I/O control device.
    /// </summary>
    public class ControlDevice : IControlDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDevice"/> class.
        /// </summary>
        /// <param name="file">A opened special file that can be controlled using ioctl-system calls.</param>
        /// <remarks><paramref name="file"/> will be disposed if the user calls <see cref="Dispose()"/> on this instance.</remarks>
        public ControlDevice(IFile file)
            : this(file, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDevice"/> class.
        /// </summary>
        /// <param name="file">A opened special file that can be controlled using ioctl-system calls.</param>
        /// <param name="disposeFile">If <c>true</c> the supplied <paramref name="file"/> will be disposed if the user calls <see cref="Dispose()"/> on this instance.</param>
        public ControlDevice(IFile file, bool disposeFile)
        {
            this.File = file;
            this.DisposeFile = disposeFile;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ControlDevice"/> class.
        /// </summary>
        ~ControlDevice()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the Device file used for communication.
        /// </summary>
        protected IFile File { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="File"/> will be disposed on <see cref="ControlDevice.Dispose()"/>.
        /// </summary>
        protected bool DisposeFile { get; }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The data to be transmitted.</param>
        /// <returns>
        /// Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.
        /// </returns>
        public int Control(uint request, ref uint data)
        {
            var result = Ioctl(this.File.Descriptor, request, ref data);
            return result;
        }

        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The data to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        public int Control(uint request, ref byte data)
        {
            var result = Ioctl(this.File.Descriptor, request, ref data);
            return result;
        }

        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">An untyped pointer to memory that contains the command/request data.</param>
        /// <returns>
        /// Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.
        /// </returns>
        public int Control(uint request, IntPtr data)
        {
            var result = Ioctl(this.File.Descriptor, request, data);
            return result;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">If <c>true</c> the managed resources will be disposed as well.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.DisposeFile)
            {
                this.File.Dispose();
            }
        }

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int Ioctl(int descriptor, uint request, IntPtr data);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int Ioctl(int descriptor, uint request, ref uint data);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int Ioctl(int descriptor, uint request, ref byte data);
    }
}