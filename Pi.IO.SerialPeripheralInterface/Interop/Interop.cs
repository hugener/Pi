// <copyright file="Interop.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System.Runtime.InteropServices;

    internal static class Interop
    {
        public const uint SpiCpha = 0x01;
        public const uint SpiCpol = 0x02;

        public const uint SpiMode0 = 0 | 0;
        public const uint SpiMode1 = 0 | SpiCpha;
        public const uint SpiMode2 = SpiCpol | 0;
        public const uint SpiMode3 = SpiCpol | SpiCpha;

        public const uint SpiCsHigh = 0x04;
        public const uint SpiLsbFirst = 0x08;
        public const uint Spi3Wire = 0x10;

        public const uint SpiLoop = 0x20;
        public const uint SpiNoCs = 0x40;
        public const uint SpiReady = 0x80;

        public const uint SpiIocMessageBase = 0x40006b00;
        public const int SpiIocMessageNumberShift = 16;

        private static readonly int TransferMessageSize = Marshal.SizeOf(typeof(SpiTransferControlStructure));

        internal static uint GetSpiMessageRequest(int numberOfMessages)
        {
            var size = unchecked((uint)(TransferMessageSize * numberOfMessages));
            return SpiIocMessageBase | (size << SpiIocMessageNumberShift);
        }
    }
}