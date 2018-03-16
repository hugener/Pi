// <copyright file="MemoryProtection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Interop
{
    using System;

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
        ReadWrite = Read | Write,
    }
}