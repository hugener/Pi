// <copyright file="Interop.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
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
        public const uint Bcm2835GpioFselOutp = 1;
        public const uint Bcm2835GpioFselAlt0 = 4;
        public const uint Bcm2835GpioFselMask = 7;

        public const uint Bcm2835Gpfsel0 = 0x0000;
        public const uint Bcm2835Gppud = 0x0094;
        public const uint Bcm2835Gppudclk0 = 0x0098;
        public const uint Bcm2835Gpset0 = 0x001c;
        public const uint Bcm2835Gpclr0 = 0x0028;
        public const uint Bcm2835Gplev0 = 0x0034;

        public const uint Bcm2835GpioPudOff = 0;
        public const uint Bcm2835GpioPudDown = 1;
        public const uint Bcm2835GpioPudUp = 2;

        public const int Epollin = 1;
        public const int Epollpri = 2;
        public const int Epollet = 1 << 31;

        public const int EpollCtlAdd = 0x1;
        public const int EpollCtlDel = 0x2;

        [DllImport("libc.so.6", EntryPoint = "epoll_create")]
        public static extern int Epoll_create(int size);

        [DllImport("libc.so.6", EntryPoint = "epoll_ctl")]
        public static extern int Epoll_ctl(int epfd, int op, int fd, IntPtr epevent);

        [DllImport("libc.so.6", EntryPoint = "epoll_wait")]
        public static extern int Epoll_wait(int epfd, IntPtr events, int maxevents, int timeout);

        [StructLayout(LayoutKind.Explicit)]
        public struct EpollData
        {
            [FieldOffset(0)]
            public IntPtr Ptr;

            [FieldOffset(0)]
            public int Fd;

            [FieldOffset(0)]
            public uint U32;

            [FieldOffset(0)]
            public ulong U64;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct EpollEvent
        {
            [FieldOffset(0)]
            public int Events;

            [FieldOffset(4)]
            public EpollData Data;
        }
    }
}