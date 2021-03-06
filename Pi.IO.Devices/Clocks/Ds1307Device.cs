﻿// <copyright file="Ds1307Device.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Devices.Clocks
{
    using global::Pi.IO.InterIntegratedCircuit;
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;

    /// <summary>
    /// Provides functionality for the DS1307 Real-Time Clock.
    /// </summary>
    /// <remarks>
    /// The Clock Module needs to be modified to work with the Raspberry Pi
    /// according to this article: http://electronics.stackexchange.com/questions/98361/how-to-modify-ds1307-rtc-to-use-3-3v-for-raspberry-pi
    /// The Datasheet can be found here: http://www.alldatasheet.com/datasheet-pdf/pdf/58481/DALLAS/DS1307.html (the Memory Map
    /// is on Page 5. Note that the values in RAM are stored in Nibbles).
    /// </remarks>
    public class Ds1307Device
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ds1307Device"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Ds1307Device(I2cDeviceConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public I2cDeviceConnection Connection { get; set; }

        /// <summary>
        /// Reads the Date and Time from the Ds1307 and returns it.
        /// </summary>
        /// <returns>Date.</returns>
        public DateTime GetDate()
        {
            return this.GetDate(this.ReadAll());
        }

        /// <summary>
        /// Writes the provided Date and Time to the Ds1307.
        /// </summary>
        /// <param name="date">The Date that should be set.</param>
        public void SetDate(DateTime date)
        {
            List<byte> toWrite = new List<byte>
            {
                0x00,
                this.SetEnabledDisableRtc(IntToNibble(date.Second), !this.IsRtcEnabled()),
                IntToNibble(date.Minute),
                IntToNibble(date.Hour),
                Convert.ToByte(date.DayOfWeek),
                IntToNibble(date.Day),
                IntToNibble(date.Month),
                IntToNibble(Convert.ToInt16(date.ToString("yy"), 10)),
            };

            this.Connection.Write(toWrite.ToArray());
        }

        /// <summary>
        /// Enables the Clock.
        /// </summary>
        public void EnableRtc()
        {
            // CH=1: Disabled, CH=0: Enabled
            byte seconds = this.ReadSeconds();
            if (this.IsRtcEnabled(seconds))
            {
                return;
            }

            this.Connection.Write(0x00, this.SetEnabledDisableRtc(seconds, false));
        }

        /// <summary>
        /// Disables the Clock. When the Clock is diabled, it not ticking.
        /// </summary>
        public void DisableRtc()
        {
            byte seconds = this.ReadSeconds();
            if (!this.IsRtcEnabled(seconds))
            {
                return;
            }

            this.Connection.Write(0x00, this.SetEnabledDisableRtc(seconds, true));
        }

        /// <summary>
        /// Returns true, if the Clock is enabled, otherwise false is returned.
        /// </summary>
        /// <returns>
        /// true: Clock is enabled, false: Clock is diabled.
        /// </returns>
        public bool IsRtcEnabled()
        {
            return this.IsRtcEnabled(this.ReadSeconds());
        }

        /// <summary>
        /// Resets the Clock to the Factory Defaults.
        /// </summary>
        public void ResetToFactoryDefaults()
        {
            // The Factory Default is: 80,00,00,01,01,01,00,B3
            this.Connection.Write(0x00, 0x80, 0x00, 0x00, 0x01, 0x01, 0x01, 0x00);
        }

        /// <summary>
        /// Writes the current System Date to the Clock.
        /// </summary>
        public void SystemTimeToRtc()
        {
            this.SetDate(DateTime.Now);
        }

        /// <summary>
        /// Converts the specified two-nibble-byte to an integer.
        /// </summary>
        /// <param name="nibble">Nibble that should be converted.</param>
        /// <returns>Integer representation of the nibble.</returns>
        private static int NibbleToInt(byte nibble)
        {
            int result = 0;
            result *= 100;
            result += 10 * (nibble >> 4);
            result += nibble & 0xf;
            return result;
        }

        /// <summary>
        /// Converts the specified integer to a two-nibble-byte.
        /// </summary>
        /// <param name="number">The integer that should be converted. Maximum is 99.</param>
        /// <returns>A byte with two nibbles that contains the integer.</returns>
        private static byte IntToNibble(int number)
        {
            int bcd = 0;
            for (int digit = 0; digit < 4; ++digit)
            {
                int nibble = number % 10;
                bcd |= nibble << (digit * 4);
                number /= 10;
            }

            return (byte)(bcd & 0xff);
        }

        /// <summary>
        /// Reads the Seconds-byte (first byte in the RAM) from the Clock and returns it.
        /// </summary>
        /// <returns>The Seconds-Byte including the CH-Flag in bit 7.</returns>
        private byte ReadSeconds()
        {
            this.Connection.Write(0x00);
            return this.Connection.ReadByte();
        }

        /// <summary>
        /// Reads 7 bytes from the Clock and returns them.
        /// </summary>
        /// <returns>7 Bytes from the Clock.</returns>
        private byte[] ReadAll()
        {
            this.Connection.Write(0x00);
            return this.Connection.Read(7);
        }

        /// <summary>
        /// Disables or enables the Clock.
        /// </summary>
        /// <param name="seconds">The byte that contains the seconds and the CH-Flag.</param>
        /// <param name="disable">true will disable the Clock, false will enable it.</param>
        /// <returns>The seconds with rtc enabled or disabled.</returns>
        /// <remarks>
        /// The disable/enabled-Flag is stored in bit 7 within the seconds-byte.
        /// </remarks>
        private byte SetEnabledDisableRtc(byte seconds, bool disable)
        {
            BitArray bits = new BitArray(new[] { seconds });
            bits.Set(7, disable);

            byte[] result = new byte[1];
            bits.CopyTo(result, 0);

            return result[0];
        }

        /// <summary>
        /// Converts the provided bytes to a DateTime.
        /// </summary>
        /// <param name="input">Bytes that should be converted.</param>
        /// <returns>DateTime resulting from the bytes.</returns>
        private DateTime GetDate(byte[] input)
        {
            /* Byte 1: CH-Flag + Seconds (00-59)
             * Byte 2: Minutes (00-59)
             * Byte 3: 12/24-Flag, AM/PM, Hours (01-12 or 00-23)
             * Byte 4: Day of week (1-7)
             * Byte 5: Day (01-31)
             * Byte 6: Month (01-12)
             * Byte 7: Year (00-99)
             * Byte 8: Control Register (for enabling/disabling Sqare Wave)
             */

            int seconds = input[0];
            if (!this.IsRtcEnabled(input[0]))
            {
                seconds = seconds - 128;  // Remove "CH"-bit from the seconds if present
            }

            seconds = NibbleToInt((byte)seconds);

            int minutes = NibbleToInt(input[1]);
            int hours = NibbleToInt(input[2]);

            if ((hours & 64) == 64)
            {
                throw new NotImplementedException("AM/PM Time is currently not supported.");
                //// 12 h Format
                //// if ((hours & 32) == 32)
                //// {
                ////     //PM
                //// }
                //// else
                //// {
                ////     //AM
                //// }
            }

            int dayOfWeek = NibbleToInt(input[3]);

            int day = NibbleToInt(input[4]);
            int month = NibbleToInt(input[5]);
            int year = NibbleToInt(input[6]);

            if (year == 0)
            {
                return default(DateTime);
            }

            return new DateTime(year + 2000, month, day, hours, minutes, seconds);
        }

        /// <summary>
        /// Returns true, if the Clock is enabled, otherwise false.
        /// </summary>
        /// <param name="seconds">The byte, that contains the seconds and the CH-Flag.</param>
        /// <returns>true: Clock is enabled, false: Clock is diabled.</returns>
        private bool IsRtcEnabled(byte seconds)
        {
            return (seconds & 128) != 128;
        }
    }
}
