// <copyright file="Ssd1306Connection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Displays.Ssd1306
{
    using Fonts;
    using global::System;
    using InterIntegratedCircuit;

    /// <summary>
    /// Represents a connection with an Ssd1306 I2C OLED display.
    /// </summary>
    public class Ssd1306Connection
    {
        private readonly object syncObject = new object();
        private readonly I2cDeviceConnection connection;
        private readonly int displayWidth;
        private readonly int displayHeight;
        private int cursorX;
        private int cursorY;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ssd1306Connection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="displayWidth">The display displayWidth.</param>
        /// <param name="displayHeight">The display displayHeight.</param>
        public Ssd1306Connection(I2cDeviceConnection connection, int displayWidth = 128, int displayHeight = 64)
        {
            this.connection = connection;
            this.displayWidth = displayWidth;
            this.displayHeight = displayHeight;
            this.Initialize();
        }

        /// <summary>
        /// Clears the screen.
        /// </summary>
        public void ClearScreen()
        {
            lock (this.syncObject)
            {
                for (var y = 0; y < this.displayWidth * this.displayHeight / 8; y++)
                {
                    this.connection.Write(0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
                }
            }
        }

        /// <summary>
        /// Inverts the display.
        /// </summary>
        public void InvertDisplay()
        {
            this.SendCommand(Command.DisplayInvert);
        }

        /// <summary>
        /// Sets the display to normal mode.
        /// </summary>
        public void NormalDisplay()
        {
            this.SendCommand(Command.DisplayNormal);
        }

        /// <summary>
        /// Turns the display on.
        /// </summary>
        public void On()
        {
            this.SendCommand(Command.DisplayOn);
        }

        /// <summary>
        /// Turns the display off.
        /// </summary>
        public void Off()
        {
            this.SendCommand(Command.DisplayOff);
        }

        /// <summary>
        /// Sets the current cursor position to the specified column and row.
        /// </summary>
        /// <param name="column">Column.</param>
        /// <param name="row">Row.</param>
        public void GotoXy(int column, int row)
        {
            this.SendCommand(
                (byte)(0xB0 + row),                         // set page address
                (byte)(0x00 + (8 * column & 0x0F)),         // set column lower address
                (byte)(0x10 + (((8 * column) >> 4) & 0x0F))); // set column higher address

            this.cursorX = column;
            this.cursorY = row;
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="font">Font.</param>
        /// <param name="text">Text.</param>
        public void DrawText(IFont font, string text)
        {
            var charset = font.GetData();
            foreach (var character in text)
            {
                var charIndex = -1;
                for (var i = 0; i < charset.Length; i++)
                {
                    if (charset[i][0] == character)
                    {
                        charIndex = i;
                        break;
                    }
                }

                if (charIndex == -1)
                {
                    continue;
                }

                var fontData = charset[charIndex];
                int fontWidth = fontData[1];
                int fontLength = fontData[2];
                for (var y = 0; y < fontLength / fontWidth; y++)
                {
                    this.SendCommand(
                        (byte)(0xB0 + this.cursorY + y), //// set page address
                        (byte)(0x00 + (this.cursorX & 0x0F)), //// set column lower address
                        (byte)(0x10 + ((this.cursorX >> 4) & 0x0F))); //// set column higher address

                    var data = new byte[fontWidth + 1];
                    data[0] = 0x40;
                    Array.Copy(fontData, (y * fontWidth) + 3, data, 1, fontWidth);
                    this.DrawStride(data);
                }

                this.cursorX += fontWidth;
            }
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="image">Image.</param>
        public void DrawImage(byte[] image)
        {
            var data = new byte[image.Length + 1];
            data[0] = 0x40;
            Array.Copy(image, 0, data, 1, image.Length);

            this.DrawStride(data);
        }

        /// <summary>
        /// Activates the scroll.
        /// </summary>
        public void ActivateScroll()
        {
            this.SendCommand(Command.ActivateScroll);
        }

        /// <summary>
        /// Deactivates the scroll.
        /// </summary>
        public void DeactivateScroll()
        {
            this.SendCommand(Command.DeactivateScroll);
        }

        /// <summary>
        /// Sets the scroll properties.
        /// </summary>
        /// <param name="direction">Direction.</param>
        /// <param name="scrollSpeed">Scroll speed.</param>
        /// <param name="startLine">Start line.</param>
        /// <param name="endLine">End line.</param>
        public void SetScrollProperties(ScrollDirection direction, ScrollSpeed scrollSpeed, int startLine, int endLine)
        {
            this.SendCommand(new byte[]
            {
                (byte)(Command.SetScrollDirection | (byte)direction),
                0x00,
                (byte)startLine,
                (byte)scrollSpeed,
                (byte)endLine,
                0x00,
                0xFF
            });
        }

        /// <summary>
        /// Sets the contrast (brightness) of the display. Default is 127.
        /// </summary>
        /// <param name="contrast">A number between 0 and 255. Contrast increases as the value increases.</param>
        public void SetContrast(int contrast)
        {
            if (contrast < 0 || contrast > 255)
            {
                throw new ArgumentOutOfRangeException("contrast", "Contrast must be between 0 and 255.");
            }

            this.SendCommand(Command.SetContrast, (byte)contrast);
        }

        private void SendCommand(params byte[] commands)
        {
            lock (this.syncObject)
            {
                foreach (byte command in commands)
                {
                    this.connection.Write(0x00, command);
                }
            }
        }

        private void Initialize()
        {
            this.SendCommand(
                Command.DisplayOff,
                Command.SetDisplayClockDivider,
                0x80,
                Command.SetMultiplex,
                0x3F,
                Command.SetDisplayOffset,
                0x00,
                Command.SetStartLine | 0x0,
                Command.ChargePump,
                0x14,
                Command.MemoryMode,
                0x00,
                Command.SegRemap | 0x1,
                Command.ComScanDecrement,
                Command.SetComPins,
                0x12,
                Command.SetContrast,
                0x7F,
                Command.SetPreCharge,
                0x22,
                Command.SetVComDetect,
                0x40,
                Command.DisplayAllOnResume,
                Command.DisplayNormal);

            this.SendCommand(
                Command.ColumnAddress,
                0,
                (byte)(this.displayWidth - 1),
                Command.PageAddress,
                0,
                (byte)((this.displayHeight / 8) - 1));

            this.ClearScreen();
        }

        private void DrawStride(byte[] data)
        {
            lock (this.syncObject)
            {
                this.connection.Write(data);
            }
        }
    }
}
