// <copyright file="Sda5708Connection.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

/* Sda5708Connection
 * Parts of this code are ported from https://github.com/pimium/sda5708
 * which in turn used the information from http://www.sbprojects.com/knowledge/footprints/sda5708.php.
 * The font is taken from http://sunge.awardspace.com/glcd-sd/node4.html
 * with additional german characters added. */
namespace Pi.IO.Components.Displays.Sda5708
{
    using GeneralPurpose;
    using global::System;
    using global::System.Threading;

    /// <summary>
    /// Represents a connection to Sda5708.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public sealed class Sda5708Connection : IDisposable
    {
        private readonly ProcessorPin load;
        private readonly ProcessorPin data;
        private readonly ProcessorPin sdclk;
        private readonly ProcessorPin reset;
        private readonly GpioConnection baseConnection;
        private Sda5708Brightness brightness = Sda5708Brightness.Level100;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sda5708Connection"/> class.
        /// </summary>
        public Sda5708Connection()
            : this(
            ProcessorPin.Pin7,
            ProcessorPin.Pin8,
            ProcessorPin.Pin18,
            ProcessorPin.Pin23)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sda5708Connection"/> class.
        /// </summary>
        /// <param name="load">The load.</param>
        /// <param name="data">The data.</param>
        /// <param name="sdclk">The SDCLK.</param>
        /// <param name="reset">The reset.</param>
        public Sda5708Connection(ProcessorPin load, ProcessorPin data, ProcessorPin sdclk, ProcessorPin reset)
        {
            this.load = load;
            this.data = data;
            this.sdclk = sdclk;
            this.reset = reset;

            this.baseConnection = new GpioConnection(
                load.Output(),
                data.Output(),
                sdclk.Output(),
                reset.Output());

            this.baseConnection[reset] = false;
            this.baseConnection[reset] = false;
            Thread.Sleep(50);
            this.baseConnection[reset] = true;

            this.Clear();
        }

        /// <summary>
        /// Sets the brightness.
        /// </summary>
        /// <param name="brightness">The brightness.</param>
        public void SetBrightness(Sda5708Brightness brightness)
        {
            this.brightness = brightness;
            this.Write(0xe0 | (int)brightness);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.Write(0xc0 | (int)this.brightness);
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="str">The string.</param>
        public void WriteString(string str)
        {
            var chars = str
                .PadRight(8, ' ')
                .Substring(0, 8);

            for (var i = 0; i < chars.Length; i++)
            {
                this.WriteChar(i, chars[i]);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.baseConnection[this.reset] = false;
            ((IDisposable)this.baseConnection).Dispose();
        }

        private void WriteChar(int position, char value)
        {
            this.Write(0xa0 + position);

            string[] pattern;
            if (!Sda5708Font.Patterns.TryGetValue(value, out pattern))
            {
                pattern = Sda5708Font.Patterns['?'];
            }

            for (var i = 0; i < 7; i++)
            {
                this.Write(Convert.ToInt32(pattern[i].Replace(' ', '0'), 2));
            }
        }

        private void Write(int value)
        {
            this.baseConnection[this.sdclk] = false;
            this.baseConnection[this.load] = false;

            for (var i = 8; i > 0; i--)
            {
                this.baseConnection[this.data] = (value & 0x1) == 0x1;

                this.baseConnection[this.sdclk] = true;
                this.baseConnection[this.sdclk] = false;

                value = value >> 1;
            }

            this.baseConnection[this.sdclk] = false;
            this.baseConnection[this.load] = true;
        }
    }
}
