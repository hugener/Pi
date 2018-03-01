// <copyright file="Command.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Displays.Ssd1306
{
    /// <summary>
    /// Commands for teh Ssd1306 display.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// The set contrast
        /// </summary>
        public const byte SetContrast = 0x81;

        /// <summary>
        /// The display all on resume
        /// </summary>
        public const byte DisplayAllOnResume = 0xA4;

        /// <summary>
        /// The display all on
        /// </summary>
        public const byte DisplayAllOn = 0xA5;

        /// <summary>
        /// The display normal
        /// </summary>
        public const byte DisplayNormal = 0xA6;

        /// <summary>
        /// The display invert
        /// </summary>
        public const byte DisplayInvert = 0xA7;

        /// <summary>
        /// The display off
        /// </summary>
        public const byte DisplayOff = 0xAE;

        /// <summary>
        /// The display on
        /// </summary>
        public const byte DisplayOn = 0xAF;

        /// <summary>
        /// The set display offset
        /// </summary>
        public const byte SetDisplayOffset = 0xD3;

        /// <summary>
        /// The set COM pins
        /// </summary>
        public const byte SetComPins = 0xDA;

        /// <summary>
        /// The set v COM detect
        /// </summary>
        public const byte SetVComDetect = 0xDB;

        /// <summary>
        /// The set display clock divider
        /// </summary>
        public const byte SetDisplayClockDivider = 0xD5;

        /// <summary>
        /// The set pre charge
        /// </summary>
        public const byte SetPreCharge = 0xD9;

        /// <summary>
        /// The set multiplex
        /// </summary>
        public const byte SetMultiplex = 0xA8;

        /// <summary>
        /// The set low column
        /// </summary>
        public const byte SetLowColumn = 0x00;

        /// <summary>
        /// The set high column
        /// </summary>
        public const byte SetHighColumn = 0x10;

        /// <summary>
        /// The set start line
        /// </summary>
        public const byte SetStartLine = 0x40;

        /// <summary>
        /// The memory mode
        /// </summary>
        public const byte MemoryMode = 0x20;

        /// <summary>
        /// The column address
        /// </summary>
        public const byte ColumnAddress = 0x21;

        /// <summary>
        /// The page address
        /// </summary>
        public const byte PageAddress = 0x22;

        /// <summary>
        /// The activate scroll
        /// </summary>
        public const byte ActivateScroll = 0x2F;

        /// <summary>
        /// The deactivate scroll
        /// </summary>
        public const byte DeactivateScroll = 0x2E;

        /// <summary>
        /// The set vertical scroll area
        /// </summary>
        public const byte SetVerticalScrollArea = 0xA3;

        /// <summary>
        /// The set scroll direction
        /// </summary>
        public const byte SetScrollDirection = 0x25;

        /// <summary>
        /// The COM scan increment
        /// </summary>
        public const byte ComScanIncrement = 0xC0;

        /// <summary>
        /// The COM scan decrement
        /// </summary>
        public const byte ComScanDecrement = 0xC8;

        /// <summary>
        /// The seg remap
        /// </summary>
        public const byte SegRemap = 0xA0;

        /// <summary>
        /// The charge pump
        /// </summary>
        public const byte ChargePump = 0x8D;

        /// <summary>
        /// The external VCC
        /// </summary>
        public const byte ExternalVcc = 0x1;

        /// <summary>
        /// The switch cap VCC
        /// </summary>
        public const byte SwitchCapVcc = 0x2;
    }
}
