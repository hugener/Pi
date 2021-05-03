// <copyright file="SpiMode.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;

    /// <summary>
    /// SPI mode.
    /// </summary>
    [Flags]
    public enum SpiMode : uint
    {
        /// <summary>
        /// Clock phase, if set CPHA=1, otherwise CPHA=0.
        /// </summary>
        ClockPhase = Interop.SpiCpha,

        /// <summary>
        /// Clock polarity, if set CPOL=1, otherwise CPOL=0.
        /// </summary>
        ClockPolarity = Interop.SpiCpol,

        /// <summary>
        /// Chip select is a high signal.
        /// </summary>
        ChipSelectActiveHigh = Interop.SpiCsHigh,

        /// <summary>
        /// The least significant bit comes first.
        /// </summary>
        LeastSignificantBitFirst = Interop.SpiLsbFirst,

        /// <summary>
        /// Special 3-wire configuration.
        /// </summary>
        ThreeWire = Interop.Spi3Wire,

        /// <summary>
        /// Three-wire serial buses.
        /// </summary>
        SlaveInOutShared = Interop.Spi3Wire,

        /// <summary>
        /// Loopback.
        /// </summary>
        Loopback = Interop.SpiLoop,

        /// <summary>
        /// Send no chip select signal.
        /// </summary>
        NoChipSelect = Interop.SpiNoCs,

        /// <summary>
        /// Slave pulls low to pause.
        /// </summary>
        Ready = Interop.SpiReady,

        /// <summary>
        /// Slave pulls low to pause.
        /// </summary>
        SlavePullsLowToPause = Interop.SpiReady,

        /// <summary>
        /// CPOL=0, CPHA=0.
        /// </summary>
        Mode0 = Interop.SpiMode0,

        /// <summary>
        /// CPOL=0, CPHA=1.
        /// </summary>
        Mode1 = Interop.SpiMode1,

        /// <summary>
        /// CPOL =1, CPHA=0.
        /// </summary>
        Mode2 = Interop.SpiMode2,

        /// <summary>
        /// CPOL=1, CPHA=1.
        /// </summary>
        Mode3 = Interop.SpiMode3,
    }
}