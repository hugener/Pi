using System;

namespace Pi.IO.Interop
{
    /// <summary>
    /// Enum which specifies whether memory is shared.
    /// </summary>
    [Flags]
    public enum MemoryFlags
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,

        /// <summary>
        /// The shared
        /// </summary>
        Shared = 1
    }
}