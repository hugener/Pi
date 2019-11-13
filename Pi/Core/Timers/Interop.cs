// <copyright file="Interop.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.Core.Timers
{
    using System;
    using System.Runtime.InteropServices;

    internal static class Interop
    {
        public const int ClockMonotonicRaw = 4;

        [DllImport("libc.so.6", EntryPoint = "nanosleep")]
        public static extern int Nanosleep(ref Timespec req, ref Timespec rem);

        public struct Timespec
        {
            public IntPtr TvSec; /* seconds */
            public IntPtr TvNsec; /* nanoseconds */
        }
    }
}