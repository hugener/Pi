﻿// <copyright file="Bmp085Device.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Sensors.Pressure.Bmp085
{
    using global::System;
    using Pi.IO.InterIntegratedCircuit;
    using Sundew.Base.Threading;

    /// <summary>
    /// Represents an I2C connection to a BMP085 barometer / thermometer.
    /// </summary>
    public class Bmp085Device
    {
        /// <summary>
        /// The default address.
        /// </summary>
        public const int DefaultAddress = 0x77;

        private static readonly TimeSpan LowDelay = TimeSpan.FromMilliseconds(5);
        private static readonly TimeSpan HighDelay = TimeSpan.FromMilliseconds(14);
        private static readonly TimeSpan HighestDelay = TimeSpan.FromMilliseconds(26);
        private static readonly TimeSpan DefaultDelay = TimeSpan.FromMilliseconds(8);
        private readonly I2cDeviceConnection connection;
        private readonly ICurrentThread thread;

        private short ac1;
        private short ac2;
        private short ac3;
        private ushort ac4;
        private ushort ac5;
        private ushort ac6;

        private short b1;
        private short b2;
        private short mb;
        private short mc;

        private short md;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bmp085Device" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="thread">The thread.</param>
        public Bmp085Device(I2cDeviceConnection connection, ICurrentThread thread)
        {
            this.connection = connection;
            this.thread = thread;
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>
        /// The precision.
        /// </value>
        public Bmp085Precision Precision { get; set; } = Bmp085Precision.Standard;

        /// <summary>
        /// Gets the pressure.
        /// </summary>
        /// <returns>The pressure.</returns>
        public UnitsNet.Pressure GetPressure()
        {
            return this.GetData().Pressure;
        }

        /// <summary>
        /// Gets the temperature.
        /// </summary>
        /// <returns>The temperature.</returns>
        public UnitsNet.Temperature GetTemperature()
        {
            // Do not use GetData here since it would imply useless I/O and computation.
            var rawTemperature = this.GetRawTemperature();
            var b5 = this.ComputeB5(rawTemperature);

            return UnitsNet.Temperature.FromDegreesCelsius((double)((b5 + 8) >> 4) / 10);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The data.</returns>
        public Bmp085Data GetData()
        {
            var rawTemperature = this.GetRawTemperature();
            var rawPressure = this.GetRawPressure();

            var b5 = this.ComputeB5(rawTemperature);

            // do pressure calcs
            var b6 = b5 - 4000;
            var x1 = (this.b2 * ((b6 * b6) >> 12)) >> 11;
            var x2 = (this.ac2 * b6) >> 11;
            var x3 = x1 + x2;
            var b3 = ((((this.ac1 * 4) + x3) << (int)this.Precision) + 2) / 4;

            x1 = (this.ac3 * b6) >> 13;
            x2 = (this.b1 * ((b6 * b6) >> 12)) >> 16;
            x3 = (x1 + x2 + 2) >> 2;
            var b4 = (this.ac4 * (uint)(x3 + 32768)) >> 15;
            var b7 = (uint)((rawPressure - b3) * (uint)(50000UL >> (int)this.Precision));

            int p;
            if (b4 == 0)
            {
                p = int.MaxValue;
            }
            else if (b7 < 0x80000000)
            {
                p = (int)(b7 * 2 / b4);
            }
            else
            {
                p = (int)(b7 / b4 * 2);
            }

            x1 = (p >> 8) * (p >> 8);
            x1 = (x1 * 3038) >> 16;
            x2 = (-7357 * p) >> 16;

            return new Bmp085Data
            {
                Pressure = UnitsNet.Pressure.FromPascals(p + ((x1 + x2 + 3791) >> 4)),
                Temperature = UnitsNet.Temperature.FromDegreesCelsius((double)((b5 + 8) >> 4) / 10),
            };
        }

        private void Initialize()
        {
            if (this.Precision > Bmp085Precision.Highest)
            {
                this.Precision = Bmp085Precision.Highest;
            }

            if (this.ReadByte(0xD0) != 0x55)
            {
                throw new InvalidOperationException("Device is not a BMP085 barometer");
            }

            /* read calibration data */
            this.ac1 = this.ReadInt16(Interop.CalAc1);
            this.ac2 = this.ReadInt16(Interop.CalAc2);
            this.ac3 = this.ReadInt16(Interop.CalAc3);
            this.ac4 = this.ReadUInt16(Interop.CalAc4);
            this.ac5 = this.ReadUInt16(Interop.CalAc5);
            this.ac6 = this.ReadUInt16(Interop.CalAc6);

            this.b1 = this.ReadInt16(Interop.CalB1);
            this.b2 = this.ReadInt16(Interop.CalB2);

            this.mb = this.ReadInt16(Interop.CalMb);
            this.mc = this.ReadInt16(Interop.CalMc);
            this.md = this.ReadInt16(Interop.CalMd);
        }

        private ushort GetRawTemperature()
        {
            this.WriteByte(Interop.Control, Interop.Readtempcmd);
            this.thread.Sleep(LowDelay);

            return this.ReadUInt16(Interop.Tempdata);
        }

        private uint GetRawPressure()
        {
            this.WriteByte(Interop.Control, (byte)(Interop.Readpressurecmd + ((int)this.Precision << 6)));

            switch (this.Precision)
            {
                case Bmp085Precision.Low:
                    this.thread.Sleep(LowDelay);
                    break;

                case Bmp085Precision.High:
                    this.thread.Sleep(HighDelay);
                    break;

                case Bmp085Precision.Highest:
                    this.thread.Sleep(HighestDelay);
                    break;

                default:
                    this.thread.Sleep(DefaultDelay);
                    break;
            }

            var msb = this.ReadByte(Interop.Pressuredata);
            var lsb = this.ReadByte(Interop.Pressuredata + 1);
            var xlsb = this.ReadByte(Interop.Pressuredata + 2);
            var raw = ((msb << 16) + (lsb << 8) + xlsb) >> (8 - (int)this.Precision);

            return (uint)raw;
        }

        private int ComputeB5(int ut)
        {
            var x1 = ((ut - this.ac6) * this.ac5) >> 15;

            if (x1 + this.md == 0)
            {
                return int.MaxValue;
            }

            var x2 = (this.mc << 11) / (x1 + this.md);
            return x1 + x2;
        }

        private byte ReadByte(byte address)
        {
            return this.ReadBytes(address, 1)[0];
        }

        private byte[] ReadBytes(byte address, int byteCount)
        {
            this.connection.WriteByte(address);
            return this.connection.Read(byteCount);
        }

        private ushort ReadUInt16(byte address)
        {
            var bytes = this.ReadBytes(address, 2);
            unchecked
            {
                return (ushort)((bytes[0] << 8) + bytes[1]);
            }
        }

        private short ReadInt16(byte address)
        {
            var bytes = this.ReadBytes(address, 2);
            unchecked
            {
                return (short)((bytes[0] << 8) + bytes[1]);
            }
        }

        private void WriteByte(byte address, byte data)
        {
            this.connection.Write(address, data);
        }

        private static class Interop
        {
            public const byte CalAc1 = 0xAA; // R   Calibration data (16 bits)
            public const byte CalAc2 = 0xAC; // R   Calibration data (16 bits)
            public const byte CalAc3 = 0xAE; // R   Calibration data (16 bits)
            public const byte CalAc4 = 0xB0; // R   Calibration data (16 bits)
            public const byte CalAc5 = 0xB2; // R   Calibration data (16 bits)
            public const byte CalAc6 = 0xB4; // R   Calibration data (16 bits)
            public const byte CalB1 = 0xB6;  // R   Calibration data (16 bits)
            public const byte CalB2 = 0xB8;  // R   Calibration data (16 bits)
            public const byte CalMb = 0xBA;  // R   Calibration data (16 bits)
            public const byte CalMc = 0xBC;  // R   Calibration data (16 bits)
            public const byte CalMd = 0xBE;  // R   Calibration data (16 bits)

            public const byte Control = 0xF4;
            public const byte Tempdata = 0xF6;
            public const byte Pressuredata = 0xF6;
            public const byte Readtempcmd = 0x2E;
            public const byte Readpressurecmd = 0x34;
        }
    }
}
