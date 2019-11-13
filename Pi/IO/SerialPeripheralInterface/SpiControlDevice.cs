// <copyright file="SpiControlDevice.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::Pi.IO.Interop;
    using global::System.Runtime.InteropServices;

    /// <summary>
    /// A Linux I/O control device that additionally can send/receive SPI data structures.
    /// </summary>
    public class SpiControlDevice : ControlDevice, ISpiControlDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpiControlDevice"/> class.
        /// </summary>
        /// <param name="file">A opened special file that can be controlled using ioctl-system calls.</param>
        /// <remarks><paramref name="file"/> will be disposed if the user calls Dispose on this instance.</remarks>
        public SpiControlDevice(IFile file)
            : base(file)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpiControlDevice"/> class.
        /// </summary>
        /// <param name="file">A opened special file that can be controlled using ioctl-system calls.</param>
        /// <param name="disposeFile">If <c>true</c> the supplied <paramref name="file"/> will be disposed if the user calls Dispose on this instance.</param>
        public SpiControlDevice(IFile file, bool disposeFile)
            : base(file, disposeFile)
        {
        }

        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The data to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        public int Control(uint request, ref SpiTransferControlStructure data)
        {
            var result = Ioctl(this.File.Descriptor, request, ref data);
            return result;
        }

        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The SPI control data structures to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        public int Control(uint request, SpiTransferControlStructure[] data)
        {
            var result = Ioctl(this.File.Descriptor, request, data);
            return result;
        }

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int Ioctl(int descriptor, uint request, ref SpiTransferControlStructure data);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int Ioctl(int descriptor, uint request, SpiTransferControlStructure[] data);
    }
}