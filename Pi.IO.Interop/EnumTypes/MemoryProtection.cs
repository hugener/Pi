using System;

namespace Pi.IO.Interop
{
    /// <summary>
    /// Enum for specifying memory access.
    /// </summary>
    [Flags]
    public enum MemoryProtection
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,

        /// <summary>
        /// The read
        /// </summary>
        Read = 1,

        /// <summary>
        /// The write
        /// </summary>
        Write = 2,

        /// <summary>
        /// The read write
        /// </summary>
        ReadWrite = Read | Write
    }
}