// <copyright file="IFile.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Interop
{
    using System;

    /// <summary>
    /// A file resource that is controlled by the underling operation system.
    /// </summary>
    public interface IFile : IDisposable
    {
        /// <summary>
        /// Gets the file descriptor
        /// </summary>
        int Descriptor { get; }

        /// <summary>
        /// Gets the pathname to the file
        /// </summary>
        string Filename { get; }
    }
}