// <copyright file="MemoryFlags.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Interop
{
    using System;

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
        Shared = 1,
    }
}