using System;
using System.Runtime.InteropServices;

namespace Pi.IO.Interop
{
    /// <summary>
    /// Wrapper methods for creating a memory map.
    /// </summary>
    public static class MemoryMap
    {
        #region Fields
        private static readonly IntPtr FAILED = new IntPtr(-1);
        #endregion

        #region Libc imports
        [DllImport("libc.so.6", EntryPoint = "mmap")]
        private static extern IntPtr mmap(IntPtr address, UIntPtr size, int protect, int flags, int file, UIntPtr offset);

        [DllImport("libc.so.6", EntryPoint = "munmap")]
        private static extern IntPtr munmap(IntPtr address, UIntPtr size);
        #endregion

        #region Methods
        /// <summary>
        /// Creates the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        /// <param name="protection">The protection.</param>
        /// <param name="memoryflags">The memoryflags.</param>
        /// <param name="fileDescriptor">The file descriptor.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public static IntPtr Create(IntPtr address, ulong size, MemoryProtection protection, MemoryFlags memoryflags, int fileDescriptor, ulong offset) {
            var result = mmap(address, new UIntPtr(size), (int) protection, (int) memoryflags, fileDescriptor, new UIntPtr(offset));
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
        /// <returns></returns>
        public static IntPtr Create(IntPtr address, uint size, MemoryProtection protection, MemoryFlags memoryflags, int fileDescriptor, uint offset) {
            var result = mmap(address, new UIntPtr(size), (int)protection, (int)memoryflags, fileDescriptor, new UIntPtr(offset));
            ThrowOnError<MemoryMapFailedException>(result);
            return result;
        }

        /// <summary>
        /// Closes the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        public static void Close(IntPtr address, ulong size) {
            var result = munmap(address, new UIntPtr(size));
            ThrowOnError<MemoryUnmapFailedException>(result);
        }

        /// <summary>
        /// Closes the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        public static void Close(IntPtr address, uint size) {
            var result = munmap(address, new UIntPtr(size));
            ThrowOnError<MemoryUnmapFailedException>(result);
        }
        #endregion

        #region Private Helpers
        private static void ThrowOnError<TException>(IntPtr result) 
            where TException: Exception, new() 
        {
            if (result == FAILED) {
                throw new TException();
            }
        }
        #endregion
    }
}