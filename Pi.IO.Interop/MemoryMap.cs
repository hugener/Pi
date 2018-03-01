// <copyright file="MemoryMap.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Interop
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Wrapper methods for creating a memory map.
    /// </summary>
    public static class MemoryMap
    {
        private static readonly IntPtr Failed = new IntPtr(-1);

        /// <summary>
        /// Creates the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        /// <param name="protection">The protection.</param>
        /// <param name="memoryflags">The memoryflags.</param>
        /// <param name="fileDescriptor">The file descriptor.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The mapping result.</returns>
        public static IntPtr Create(IntPtr address, ulong size, MemoryProtection protection, MemoryFlags memoryflags, int fileDescriptor, ulong offset)
        {
            var result = Mmap(address, new UIntPtr(size), (int)protection, (int)memoryflags, fileDescriptor, new UIntPtr(offset));
            ThrowOnError<MemoryMapFailedException>(result);
            return result;
        }

        /// <summary>
        /// Creates the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        /// <param name="protection">The protection.</param>
        /// <param name="memoryflags">The memoryflags.</param>
        /// <param name="fileDescriptor">The file descriptor.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The map result.</returns>
        public static IntPtr Create(IntPtr address, uint size, MemoryProtection protection, MemoryFlags memoryflags, int fileDescriptor, uint offset)
        {
            var result = Mmap(address, new UIntPtr(size), (int)protection, (int)memoryflags, fileDescriptor, new UIntPtr(offset));
            ThrowOnError<MemoryMapFailedException>(result);
            return result;
        }

        /// <summary>
        /// Closes the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        public static void Close(IntPtr address, ulong size)
        {
            var result = Munmap(address, new UIntPtr(size));
            ThrowOnError<MemoryUnmapFailedException>(result);
        }

        /// <summary>
        /// Closes the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        public static void Close(IntPtr address, uint size)
        {
            var result = Munmap(address, new UIntPtr(size));
            ThrowOnError<MemoryUnmapFailedException>(result);
        }

        [DllImport("libc.so.6", EntryPoint = "mmap")]
        private static extern IntPtr Mmap(IntPtr address, UIntPtr size, int protect, int flags, int file, UIntPtr offset);

        [DllImport("libc.so.6", EntryPoint = "munmap")]
        private static extern IntPtr Munmap(IntPtr address, UIntPtr size);

        private static void ThrowOnError<TException>(IntPtr result)
            where TException : Exception, new()
        {
            if (result == Failed)
            {
                throw new TException();
            }
        }
    }
}