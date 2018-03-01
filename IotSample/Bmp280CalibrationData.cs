// <copyright file="Bmp280CalibrationData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IotSample
{
    public sealed class Bmp280CalibrationData
    {
        // BMP280 Registers
        public ushort DigT1 { get; set; }

        public short DigT2 { get; set; }

        public short DigT3 { get; set; }

        public ushort DigP1 { get; set; }

        public short DigP2 { get; set; }

        public short DigP3 { get; set; }

        public short DigP4 { get; set; }

        public short DigP5 { get; set; }

        public short DigP6 { get; set; }

        public short DigP7 { get; set; }

        public short DigP8 { get; set; }

        public short DigP9 { get; set; }
    }
}