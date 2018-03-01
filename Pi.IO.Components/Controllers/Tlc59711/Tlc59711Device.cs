// <copyright file="Tlc59711Device.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Controllers.Tlc59711
{
    using global::System;
    using Interop;

    /// <summary>
    /// Adafruit 12-channel 16bit PWM/LED driver TLC59711
    /// </summary>
    public class Tlc59711Device : ITlc59711Device
    {
        /// <summary>
        /// The command size.
        /// </summary>
        public const int CommandSize = 28;
        private const byte MagicWord = 0x25 << 2;

        private const byte Outtmg = 1 << 1;
        private const byte Extgck = 1;
        private const byte Tmgrst = 1 << 7;
        private const byte Dsprpt = 1 << 6;
        private const byte BlankCode = 1 << 5;
        private const byte Off = 0;
        private const int DataOffset = 4;
        private const int DataLength = CommandSize - DataOffset;

        private readonly IMemory memory;
        private readonly ITlc59711Settings initSettings;
        private readonly IPwmChannels channels;

        private byte referenceClockEdge = Outtmg;
        private byte referenceClock = Off;
        private byte displayTimingResetMode = Tmgrst;
        private byte displayRepeatMode = Dsprpt;
        private byte blank = BlankCode;
        private byte bcb = 127;
        private byte bcg = 127;
        private byte bcr = 127;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tlc59711Device"/> class.
        /// </summary>
        /// <param name="memory">The memory.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException">memory</exception>
        /// <exception cref="ArgumentException">thrown if memory is too small.</exception>
        public Tlc59711Device(IMemory memory, ITlc59711Settings settings = null)
        {
            if (ReferenceEquals(memory, null))
            {
                throw new ArgumentNullException(nameof(memory));
            }

            if (memory.Length < CommandSize)
            {
                throw new ArgumentException(
                    string.Format("Need at least {0} bytes, got {1} bytes.", CommandSize, memory.Length), nameof(memory));
            }

            this.memory = memory;
            this.initSettings = settings;
            this.channels = new Tlc59711Channels(memory, DataOffset);

            this.Reset();
        }

        /// <summary>
        /// Gets or sets a value indicating whether all outputs are forced off.
        /// </summary>
        public bool Blank
        {
            get => this.blank == BlankCode;
            set
            {
                this.blank = value ? BlankCode : Off;
                this.WriteSecondByte();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether display is in auto display repeat mode.
        /// </summary>
        /// <remarks>
        /// Each constant-current output is only turned on once, according the GS data after
        /// <see cref="ITlc59711Settings.Blank"/> is set to <c>false</c> or after the internal latch pulse is
        /// generated with <see cref="ITlc59711Settings.DisplayTimingResetMode"/> set to <c>true</c>. If <c>true</c>
        /// each output turns on and off according to the GS data every 65536 GS reference clocks.
        /// </remarks>
        public bool DisplayRepeatMode
        {
            get => this.displayRepeatMode == Dsprpt;
            set
            {
                this.displayRepeatMode = value ? Dsprpt : Off;
                this.WriteSecondByte();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the display timing reset mode.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, the GS counter is reset to '0' and all constant-current
        /// outputs are forced off when the internal latch pulse is generated for data latching.
        /// This function is the same when <see cref="ITlc59711Settings.Blank"/> is set to <c>false</c>.
        /// Therefore, <see cref="ITlc59711Settings.Blank"/> does not need to be controlled by an external controller
        /// when this mode is enabled. If <c>false</c>, the GS counter is not reset and no output
        /// is forced off even if the internal latch pulse is generated.
        /// </remarks>
        public bool DisplayTimingResetMode
        {
            get => this.displayTimingResetMode == Tmgrst;
            set
            {
                this.displayTimingResetMode = value ? Tmgrst : Off;
                this.WriteSecondByte();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the reference clock selection.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, PWM timing refers to the SCKI clock. If <c>false</c>, PWM timing
        /// refers to the internal oscillator clock.
        /// </remarks>
        public bool ReferenceClock
        {
            get => this.referenceClock == Extgck;
            set
            {
                this.referenceClock = value ? Extgck : Off;
                this.WriteFirstByte();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the GS reference clock edge selection for OUTXn on-off timing control is enabled.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, OUTXn are turned on or off at the rising edge of the selected GS reference clock.
        /// If <c>false</c>, OUTXn are turned on or off at the falling edge of the selected clock.
        /// </remarks>
        public bool ReferenceClockEdge
        {
            get => this.referenceClockEdge == Outtmg;
            set
            {
                this.referenceClockEdge = value ? Outtmg : Off;
                this.WriteFirstByte();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the Global brightness control for OUTR0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current.
        /// </remarks>
        public byte BrightnessControlR
        {
            get => this.bcr;
            set
            {
                value.ThrowOnInvalidBrightnessControl();

                this.bcr = value;
                this.WriteFourthByte();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the Global brightness control for OUTG0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current.
        /// </remarks>
        public byte BrightnessControlG
        {
            get => this.bcg;
            set
            {
                value.ThrowOnInvalidBrightnessControl();

                this.bcg = value;
                this.WriteThirdByte();
                this.WriteFourthByte();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the Global brightness control for OUTB0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current.
        /// </remarks>
        public byte BrightnessControlB
        {
            get => this.bcb;
            set
            {
                value.ThrowOnInvalidBrightnessControl();

                this.bcb = value;
                this.WriteSecondByte();
                this.WriteThirdByte();
            }
        }

        /// <summary>
        /// Gets the PWM channels.
        /// </summary>
        public IPwmChannels Channels => this.channels;

        /// <summary>
        /// Initializes the device with default values.
        /// </summary>
        public void Reset()
        {
            this.Initialize(this.initSettings ?? new Tlc59711Settings());
        }

        private void Initialize(ITlc59711Settings settings)
        {
            this.referenceClockEdge = settings.ReferenceClockEdge ? Outtmg : Off;
            this.referenceClock = settings.ReferenceClock ? Extgck : Off;
            this.displayTimingResetMode = settings.DisplayTimingResetMode ? Tmgrst : Off;
            this.displayRepeatMode = settings.DisplayRepeatMode ? Dsprpt : Off;
            this.blank = settings.Blank ? BlankCode : Off;
            this.bcb = settings.BrightnessControlB;
            this.bcg = settings.BrightnessControlG;
            this.bcr = settings.BrightnessControlR;

            this.WriteFirstByte();
            this.WriteSecondByte();
            this.WriteThirdByte();
            this.WriteFourthByte();

            var zero = new byte[DataLength];
            this.memory.Copy(zero, 0, DataOffset, DataLength);
        }

        private void WriteFirstByte()
        {
            this.memory.Write(0, (byte)(MagicWord | this.referenceClockEdge | this.referenceClock));
        }

        private void WriteSecondByte()
        {
            this.memory.Write(1, (byte)(this.displayTimingResetMode | this.displayRepeatMode | this.blank | (this.bcb >> 2)));
        }

        private void WriteThirdByte()
        {
            this.memory.Write(2, (byte)((this.bcb << 6) | (this.bcg >> 1)));
        }

        private void WriteFourthByte()
        {
            this.memory.Write(3, (byte)((this.bcg << 7) | this.bcr));
        }
    }
}