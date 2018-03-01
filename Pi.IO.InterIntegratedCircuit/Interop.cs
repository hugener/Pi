// <copyright file="Interop.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.InterIntegratedCircuit
{
    using global::System;
    using global::System.Runtime.InteropServices;

    internal static class Interop
    {
        public const uint Bcm2835PeriBase = 0x20000000;
        public const uint Bcm2835GpioBase = Bcm2835PeriBase + 0x200000;
        public const uint Bcm2835Bsc0Base = Bcm2835PeriBase + 0x205000;
        public const uint Bcm2835Bsc1Base = Bcm2835PeriBase + 0x804000;

        public const uint Bcm2836PeriBase = 0x3F000000;
        public const uint Bcm2836GpioBase = Bcm2836PeriBase + 0x200000;
        public const uint Bcm2836Bsc0Base = Bcm2836PeriBase + 0x205000;
        public const uint Bcm2836Bsc1Base = Bcm2836PeriBase + 0x804000;

        public const uint Bcm2835BlockSize = 4 * 1024;

        public const uint Bcm2835BscC = 0x0000;
        public const uint Bcm2835BscS = 0x0004;
        public const uint Bcm2835BscDlen = 0x0008;
        public const uint Bcm2835BscA = 0x000c;
        public const uint Bcm2835BscFifo = 0x0010;
        public const uint Bcm2835BscDiv = 0x0014;

        public const uint Bcm2835BscCClear1 = 0x00000020;
        public const uint Bcm2835BscCClear2 = 0x00000010;
        public const uint Bcm2835BscCI2Cen = 0x00008000;
        public const uint Bcm2835BscCSt = 0x00000080;
        public const uint Bcm2835BscCRead = 0x00000001;

        public const uint Bcm2835BscSClkt = 0x00000200;
        public const uint Bcm2835BscSErr = 0x00000100;
        public const uint Bcm2835BscSDone = 0x00000002;
        public const uint Bcm2835BscSTxd = 0x00000010;
        public const uint Bcm2835BscSRxd = 0x00000020;

        public const uint Bcm2835BscFifoSize = 16;

        public const uint Bcm2835CoreClkHz = 250000000;

        public const uint Bcm2835GpioFselInpt = 0;
        public const uint Bcm2835GpioFselAlt0 = 4;
        public const uint Bcm2835GpioFselMask = 7;

        public const uint Bcm2835Gpfsel0 = 0x0000;

        public const int ORdwr = 2;
        public const int OSync = 10000;

        public const int ProtRead = 1;
        public const int ProtWrite = 2;

        public const int MapShared = 1;
        public const int MapFailed = -1;

        [DllImport("libc.so.6", EntryPoint = "open")]
        public static extern IntPtr Open(string fileName, int mode);

        [DllImport("libc.so.6", EntryPoint = "close")]
        public static extern void Close(IntPtr file);

        [DllImport("libc.so.6", EntryPoint = "mmap")]
        public static extern IntPtr Mmap(IntPtr address, uint size, int protect, int flags, IntPtr file, uint offset);

        [DllImport("libc.so.6", EntryPoint = "munmap")]
        public static extern IntPtr Munmap(IntPtr address, uint size);
    }
}