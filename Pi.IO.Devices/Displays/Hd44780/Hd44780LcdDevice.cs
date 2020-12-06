// <copyright file="Hd44780LcdDevice.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics;
using Pi.Core.Threading;
using Pi.Core.Timers;

namespace Pi.IO.Devices.Displays.Hd44780
{
    using global::Pi.IO.GeneralPurpose;
    using global::System;
    using global::System.Text;
    using Sundew.Base.Threading;

    /// <summary>
    /// Based on https://github.com/adafruit/Adafruit-Raspberry-Pi-Python-Code/blob/master/Adafruit_CharLCD/Adafruit_CharLCD.py
    ///      and http://lcd-linux.sourceforge.net/pdfdocs/hd44780.pdf
    ///      and http://www.quinapalus.com/hd44780udg.html
    ///      and http://robo.fe.uni-lj.si/~kamnikr/sola/urac/vaja3_display/How%20to%20control%20HD44780%20display.pdf
    ///      and http://web.stanford.edu/class/ee281/handouts/lcd_tutorial.pdf
    ///      and http://www.systronix.com/access/Systronix_20x4_lcd_brief_data.pdf.
    /// </summary>
    public class Hd44780LcdDevice : IDisposable
    {
        private const int MaxHeight = 4;   // Allow for larger displays
        private const int MaxChar = 80;    // This allows for setups such as 40x2 or a 20x4

        private readonly TimeSpan syncDelay;
        private readonly IGpioConnectionDriverFactory gpioConnectionDriverFactory;
        private readonly IGpioConnectionDriver gpioConnectionDriver;
        private readonly Hd44780Pins pins;
        private readonly ICurrentThread thread;

        private readonly int width;
        private readonly int height;

        private readonly Functions functions;
        private readonly Encoding encoding;
        private readonly EntryModeFlags entryModeFlags;

        private DisplayFlags displayFlags = DisplayFlags.DisplayOn | DisplayFlags.BlinkOff | DisplayFlags.CursorOff;
        private Hd44780Position currentPosition;

        private bool backlightEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdDevice"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="gpioConnectionDriverFactory">The gpio connection driver factory.</param>
        /// <param name="registerSelectPin">The register select pin.</param>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="hd44780DataPins">The HD44780 data pins.</param>
        /// <param name="backlight">The backlight.</param>
        /// <param name="readWrite">The read write.</param>
        /// <param name="threadFactory">The thread factory.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// pins - There must be either 4 or 8 data pins
        /// or
        /// settings - ScreenHeight must be between 1 and 4 rows
        /// or
        /// settings - PatternWidth must be 5 pixels
        /// or
        /// settings - PatternWidth must be either 7 or 10 pixels height.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// At most 80 characters are allowed
        /// or
        /// 10 pixels height pattern cannot be used with an even number of rows.
        /// </exception>
        public Hd44780LcdDevice(
            Hd44780LcdDeviceSettings settings,
            IGpioConnectionDriverFactory gpioConnectionDriverFactory,
            ConnectorPin registerSelectPin,
            ConnectorPin clockPin,
            Hd44780DataPins hd44780DataPins,
            ConnectorPin? backlight = null,
            ConnectorPin? readWrite = null,
            IThreadFactory threadFactory = null)
        {
            this.gpioConnectionDriverFactory = GpioConnectionDriverFactory.EnsureGpioConnectionDriverFactory(gpioConnectionDriverFactory);
            this.gpioConnectionDriver = gpioConnectionDriverFactory.Get();
            this.pins = new Hd44780Pins(this.gpioConnectionDriver, registerSelectPin, clockPin, backlight, readWrite, hd44780DataPins.ConnectorPins);
            this.thread = ThreadFactory.EnsureThreadFactory(threadFactory).Create();
            this.syncDelay = settings.SyncDelay;

            if (this.pins.Data.Length != 4 && this.pins.Data.Length != 8)
            {
                throw new ArgumentOutOfRangeException(nameof(hd44780DataPins), this.pins.Data.Length, "There must be either 4 or 8 data pins");
            }

            this.width = settings.ScreenWidth;
            this.height = settings.ScreenHeight;
            if (this.height < 1 || this.height > MaxHeight)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.ScreenHeight), this.height, "ScreenHeight must be between 1 and 4 rows");
            }

            if (this.width * this.height > MaxChar)
            {
                throw new ArgumentException("At most 80 characters are allowed");
            }

            if (settings.PatternWidth != 5)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.PatternWidth), settings.PatternWidth, "PatternWidth must be 5 pixels");
            }

            if (settings.PatternHeight != 8 && settings.PatternHeight != 10)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.PatternHeight), settings.PatternHeight, "PatternHeight must be either 7 or 10 pixels height");
            }

            if (settings.PatternHeight == 10 && this.height % 2 == 0)
            {
                throw new ArgumentException("10 pixels height pattern cannot be used with an even number of rows");
            }

            this.functions = (settings.PatternHeight == 8 ? Functions.Matrix5X8 : Functions.Matrix5X10)
                | (this.height == 1 ? Functions.OneLine : Functions.TwoLines)
                | (this.pins.Data.Length == 4 ? Functions.Data4Bits : Functions.Data8Bits);

            this.entryModeFlags = /*settings.RightToLeft
                ? EntryModeFlags.EntryRight | EntryModeFlags.EntryShiftDecrement
                :*/ EntryModeFlags.EntryLeft | EntryModeFlags.EntryShiftDecrement;

            this.encoding = settings.Encoding;

            this.BacklightEnabled = false;

            this.pins.ReadWrite?.Write(false);

            this.pins.RegisterSelect.Write(false);
            this.pins.Clock.Write(false);
            foreach (var dataPin in this.pins.Data)
            {
                dataPin.Write(false);
            }

            this.WriteByte(0x33, false); // Initialize
            this.WriteByte(0x32, false);

            this.WriteCommand(Command.SetFunctions, (int)this.functions);
            this.WriteCommand(Command.SetDisplayFlags, (int)this.displayFlags);
            this.WriteCommand(Command.SetEntryModeFlags, (int)this.entryModeFlags);

            this.Clear();
            this.BacklightEnabled = true;
        }

        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <value>
        /// The cursor position.
        /// </value>
        public Hd44780Position CursorPosition => this.currentPosition;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is clearing on close.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is clearing on close; otherwise, <c>false</c>.
        /// </value>
        public bool IsClearingOnClose { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether display is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if display is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayEnabled
        {
            get => (this.displayFlags & DisplayFlags.DisplayOn) == DisplayFlags.DisplayOn;
            set
            {
                if (value)
                {
                    this.displayFlags |= DisplayFlags.DisplayOn;
                }
                else
                {
                    this.displayFlags &= ~DisplayFlags.DisplayOn;
                }

                this.WriteCommand(Command.SetDisplayFlags, (int)this.displayFlags);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has backlight.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has backlight; otherwise, <c>false</c>.
        /// </value>
        public bool HasBacklight => this.pins.Backlight != null;

        /// <summary>
        /// Gets or sets a value indicating whether backlight is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if backlight is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool BacklightEnabled
        {
            get => this.backlightEnabled;
            set
            {
                if (this.HasBacklight)
                {
                    this.pins.Backlight.Write(value);
                    this.backlightEnabled = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cursor is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cursor is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CursorEnabled
        {
            get => (this.displayFlags & DisplayFlags.CursorOn) == DisplayFlags.CursorOn;
            set
            {
                if (value)
                {
                    this.displayFlags |= DisplayFlags.CursorOn;
                }
                else
                {
                    this.displayFlags &= ~DisplayFlags.CursorOn;
                }

                this.WriteCommand(Command.SetDisplayFlags, (int)this.displayFlags);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cursor is blinking.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cursor is blinking; otherwise, <c>false</c>.
        /// </value>
        public bool CursorBlinking
        {
            get => (this.displayFlags & DisplayFlags.BlinkOn) == DisplayFlags.BlinkOn;
            set
            {
                if (value)
                {
                    this.displayFlags |= DisplayFlags.BlinkOn;
                }
                else
                {
                    this.displayFlags &= ~DisplayFlags.BlinkOn;
                }

                this.WriteCommand(Command.SetDisplayFlags, (int)this.displayFlags);
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (this.IsClearingOnClose)
            {
                this.Clear();
            }

            this.pins.Dispose();
            this.gpioConnectionDriverFactory.Dispose();
        }

        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            var oneMillisecond = TimeSpan.FromMilliseconds(2);
            this.pins.RegisterSelect.Write(false);

            this.pins.Data[0].Write(true);
            this.pins.Data[1].Write(true);
            this.pins.Data[2].Write(false);
            this.pins.Data[3].Write(false);

            var stopwatch = Stopwatch.StartNew();
            this.Synchronize(TimeSpan.FromMilliseconds(20), oneMillisecond);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);

            this.pins.Data[0].Write(true);
            this.pins.Data[1].Write(true);
            this.pins.Data[2].Write(false);
            this.pins.Data[3].Write(false);

            stopwatch.Restart();
            this.Synchronize(oneMillisecond, oneMillisecond);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);

            this.pins.Data[0].Write(true);
            this.pins.Data[1].Write(true);
            this.pins.Data[2].Write(false);
            this.pins.Data[3].Write(false);

            stopwatch.Restart();
            this.Synchronize(oneMillisecond, oneMillisecond);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);

            if (pins.Data.Length == 4)
            {
                this.pins.Data[0].Write(false);
                this.pins.Data[1].Write(true);
                this.pins.Data[2].Write(false);
                this.pins.Data[3].Write(false);
            }

            stopwatch.Restart();
            this.Synchronize(oneMillisecond, oneMillisecond);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);

            this.WriteCommand(Command.SetFunctions, (int)this.functions);
            this.WriteCommand(Command.SetDisplayFlags, (int)this.displayFlags);
            this.WriteCommand(Command.SetEntryModeFlags, (int)this.entryModeFlags);

            this.Clear();
        }

        /// <summary>
        /// Set cursor to top left corner.
        /// </summary>
        public void Home()
        {
            this.WriteCommand(Command.ReturnHome);
            this.currentPosition = Hd44780Position.Zero;
            this.thread.Sleep(TimeSpan.FromMilliseconds(6));
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        public void Clear()
        {
            this.WriteCommand(Command.ClearDisplay);
            this.currentPosition = Hd44780Position.Zero;

            this.thread.Sleep(TimeSpan.FromMilliseconds(6)); // Clearing the display takes a long time
        }

        /// <summary>
        /// Moves the cursor of the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Move(int offset)
        {
            var column = this.currentPosition.Column += offset;
            var row = this.currentPosition.Row;

            if (column >= this.width)
            {
                column = 0;
                row++;
            }
            else if (column < 0)
            {
                column = this.width - 1;
                row--;
            }

            if (row >= this.height)
            {
                row = 0;
            }

            if (row < 0)
            {
                row = this.height - 1;
            }

            this.SetCursorPosition(new Hd44780Position { Row = row, Column = column });
        }

        /// <summary>
        /// Moves the cursor to the specified row and column.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetCursorPosition(Hd44780Position position)
        {
            var row = position.Row;
            if (row < 0 || this.height <= row)
            {
                row = this.height - 1;
            }

            var column = position.Column;
            if (column < 0 || this.width <= column)
            {
                column = this.width - 1;
            }

            var address = column + this.GetLcdAddressLocation(row);

            this.WriteByte(address, false);

            this.currentPosition = new Hd44780Position { Row = row, Column = column };
        }

        /// <summary>
        /// Sets the custom character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="pattern">The pattern.</param>
        public void SetCustomCharacter(byte character, byte[] pattern)
        {
            if ((this.functions & Functions.Matrix5X8) == Functions.Matrix5X8)
            {
                this.Set5X8CustomCharacter(character, pattern);
            }
            else
            {
                this.Set5X10CustomCharacter(character, pattern);
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void WriteLine(object value, TimeSpan animationDelay = default)
        {
            this.WriteLine("{0}", value, animationDelay);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void WriteLine(string text, TimeSpan animationDelay = default)
        {
            this.Write(text + Environment.NewLine, animationDelay);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void Write(object value, TimeSpan animationDelay = default)
        {
            this.Write("{0}", value, animationDelay);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        public void WriteLine(string format, params object[] values)
        {
            this.WriteLine(string.Format(format, values));
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        public void Write(string format, params object[] values)
        {
            this.Write(string.Format(format, values));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="animationDelay">The animation delay.</param>
        /// <param name="values">The values.</param>
        public void WriteLine(string format, TimeSpan animationDelay, params object[] values)
        {
            this.WriteLine(string.Format(format, values), animationDelay);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="animationDelay">The animation delay.</param>
        /// <param name="values">The values.</param>
        public void Write(string format, TimeSpan animationDelay, params object[] values)
        {
            this.Write(string.Format(format, values), animationDelay);
        }

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void Write(string text, TimeSpan animationDelay = default)
        {
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var bytes = this.encoding.GetBytes(line);
                foreach (var b in bytes)
                {
                    if (this.currentPosition.Column < this.width)
                    {
                        this.WriteByte(b, true);
                    }

                    if (animationDelay > TimeSpan.Zero)
                    {
                        this.thread.Sleep(animationDelay);
                    }

                    this.currentPosition.Column++;
                }

                if ((this.currentPosition.Row == 0 || (this.currentPosition.Row + 1) % this.height != 0) && this.height > 1)
                {
                    var addressLocation = this.GetLcdAddressLocation(this.currentPosition.Row + 1);

                    this.WriteByte(addressLocation, false);
                    this.currentPosition.Column = 0;
                    this.currentPosition.Row++;
                }
                else
                {
                    this.Home(); // This was added to return home when the maximum number of row's has been achieved.
                    break;
                }
            }
        }

        private void WriteCommand(Command command, int parameter = 0)
        {
            var bits = (int)command | parameter;
            this.WriteByte(bits, false);
        }

        private void Set5X10CustomCharacter(byte character, byte[] pattern)
        {
            if (character > 7 || (character & 0x1) != 0x1)
            {
                throw new ArgumentOutOfRangeException(nameof(character), character, "character must be lower or equal to 7, and not an odd number");
            }

            if (pattern.Length != 10)
            {
                throw new ArgumentOutOfRangeException(nameof(pattern), pattern, "pattern must be 10 rows long");
            }

            this.WriteCommand(Command.SetCgRamAddr, character << 3);
            for (var i = 0; i < 10; i++)
            {
                this.WriteByte(pattern[i], true);
            }

            this.WriteByte(0, true);
        }

        private void Set5X8CustomCharacter(byte character, byte[] pattern)
        {
            if (character > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(character), character, "character must be lower or equal to 7");
            }

            if (pattern.Length != 7)
            {
                throw new ArgumentOutOfRangeException(nameof(pattern), pattern, "pattern must be 7 rows long");
            }

            this.WriteCommand(Command.SetCgRamAddr, character << 3);
            for (var i = 0; i < 7; i++)
            {
                this.WriteByte(pattern[i], true);
            }

            this.WriteByte(0, true);
        }

        private void WriteByte(int bits, bool charMode)
        {
            if (this.pins.Data.Length == 4)
            {
                this.WriteByte4Pins(bits, charMode);
            }
            else
            {
                throw new NotImplementedException("8 bits mode is currently not implemented");
            }
        }

        private void WriteByte4Pins(int bits, bool charMode)
        {
            this.pins.RegisterSelect.Write(charMode);

            this.pins.Data[0].Write((bits & 0x10) != 0);
            this.pins.Data[1].Write((bits & 0x20) != 0);
            this.pins.Data[2].Write((bits & 0x40) != 0);
            this.pins.Data[3].Write((bits & 0x80) != 0);

            this.Synchronize(this.syncDelay, this.syncDelay);

            this.pins.Data[0].Write((bits & 0x01) != 0);
            this.pins.Data[1].Write((bits & 0x02) != 0);
            this.pins.Data[2].Write((bits & 0x04) != 0);
            this.pins.Data[3].Write((bits & 0x08) != 0);

            this.Synchronize(this.syncDelay, this.syncDelay);
        }

        /// <summary>
        /// Returns the Lcd Address for the given row.
        /// </summary>
        /// <param name="row">A zero based row position.</param>
        /// <returns>The Lcd Address as an int.</returns>
        /// <remarks>http://www.mikroe.com/forum/viewtopic.php?t=5149.</remarks>
        private int GetLcdAddressLocation(int row)
        {
            const int baseAddress = 128;

            switch (row)
            {
                case 0: return baseAddress;
                case 1: return baseAddress + 64;
                case 2: return baseAddress + this.width;
                case 3: return baseAddress + 64 + this.width;
                default: return baseAddress;
            }
        }

        private void Synchronize(TimeSpan syncDelay, TimeSpan postDelay)
        {
            this.pins.Clock.Write(true);
            this.thread.Sleep(syncDelay); // 1 microsecond pause - enable pulse must be > 450ns

            this.pins.Clock.Write(false);
            this.thread.Sleep(postDelay); // commands need > 37us to settle
        }
    }
}