// <copyright file="UnixFile.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Interop
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A UNIX file that is controlled by the operation system.
    /// </summary>
    public sealed class UnixFile : IFile
    {
        private int descriptor;
        private string filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnixFile"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="fileMode">The file mode.</param>
        public UnixFile(string filename, UnixFileMode fileMode)
            : this(OpenFileDescriptor(filename, fileMode))
        {
            this.filename = filename;
        }

        private UnixFile(int descriptor)
        {
            this.descriptor = descriptor;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="UnixFile"/> class.
        /// </summary>
        ~UnixFile()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the file descriptor.
        /// </summary>
        public int Descriptor => this.descriptor;

        /// <summary>
        /// Gets the pathname for the file.
        /// </summary>
        public string Filename => this.filename;

        /// <summary>
        /// Opens a UNIX file.
        /// </summary>
        /// <param name="fileName">The filepath.</param>
        /// <param name="fileMode">The file access mode.</param>
        /// <returns>A opened file.</returns>
        public static IFile Open(string fileName, UnixFileMode fileMode)
        {
            return new UnixFile(fileName, fileMode);
        }

        /// <summary>
        /// Opens a UNIX file and returns the file descriptor.
        /// </summary>
        /// <param name="fileName">The filepath.</param>
        /// <param name="fileMode">The file access mode.</param>
        /// <returns>The file descriptor returned by a successful call will be the lowest-numbered file descriptor not currently open for the process.</returns>
        public static int OpenFileDescriptor(string fileName, UnixFileMode fileMode)
        {
            var mode = (int)fileMode;
            return Open(fileName, mode);
        }

        /// <summary>
        /// Closes a file descriptor, so that it no longer refers to any file and may be reused. Any record locks held on the file it was associated with, and owned by
        /// the process, are removed (regardless of the file descriptor that was used to obtain the lock).
        /// </summary>
        /// <param name="fileDescriptor">The file descriptor the shall be closed.</param>
        /// <returns><c>true</c> on success.</returns>
        /// <remarks>
        /// If <paramref name="fileDescriptor"/> is the last file descriptor referring to the underlying open file description, the resources associated with
        /// the open file description are freed; if the descriptor was the last reference to a file which has been removed using unlink the file is deleted.
        /// </remarks>
        public static bool CloseFileDescriptor(int fileDescriptor)
        {
            if (fileDescriptor != 0)
            {
                return Close(fileDescriptor) == 0;
            }

            return false;
        }

        /// <summary>
        /// Closes the file and frees all unmanaged system resources. See <see cref="CloseFileDescriptor"/> for more information.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [DllImport("libc.so.6", EntryPoint = "open")]
        private static extern int Open(string fileName, int mode);

        [DllImport("libc.so.6", EntryPoint = "close")]
        private static extern int Close(int file);

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed here
            }

            Trace.Assert(
                disposing,
                string.Format("ERROR: GC finalized a unix file '{0}' with open file descriptor {1} that was not disposed!", this.filename, this.descriptor));

            if (this.descriptor != 0)
            {
                // we need to free unmanaged resources to avoid memory leeks
                Close(this.descriptor);
                this.descriptor = 0;
                this.filename = null;
            }
        }
    }
}