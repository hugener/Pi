// <copyright file="I2CNativeLib.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace I2C.Net
{
    using System.Runtime.InteropServices;

    internal static class I2CNativeLib
    {
        internal static int OpenReadWrite = 2;

        // constant, even for different devices
        internal static int I2CSlave = 0x0703;

        [DllImport("libc.so.6", EntryPoint = "open")]
        internal static extern int Open(string fileName, int mode);

        [DllImport("libc.so.6", EntryPoint = "close", SetLastError = true)]
        internal static extern int Close(int busHandle);

        [DllImport("libc.so.6", EntryPoint = "ioctl", SetLastError = true)]
        internal static extern int Ioctl(int fd, int request, int data);

        [DllImport("libc.so.6", EntryPoint = "read", SetLastError = true)]
        internal static extern int Read(int handle, byte[] data, int length);

        [DllImport("libc.so.6", EntryPoint = "write", SetLastError = true)]
        internal static extern int Write(int handle, byte[] data, int length);

        /*TODO: Move these to C# based on the above.


        [DllImport("libnativei2c.so", EntryPoint = "readBytes", SetLastError = true)]
        internal static extern int ReadBytes(int busHandle, int addr, byte[] buf, int len);

        */
    }
}