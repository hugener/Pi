// <copyright file="ITlc59711Settings.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Controllers.Tlc59711
{
    /// <summary>
    /// TLC59711 settings
    /// </summary>
    public interface ITlc59711Settings
    {
        /// <summary>
        /// Gets or sets a value indicating whether all outputs are forced off. Default value is <c>true</c>.
        /// </summary>
        bool Blank { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the auto display repeat mode is enabled.
        /// </summary>
        /// <remarks>
        /// Each constant-current output is only turned on once, according the GS data after
        /// <see cref="Blank"/> is set to <c>false</c> or after the internal latch pulse is
        /// generated with <see cref="DisplayTimingResetMode"/> set to <c>true</c>. If <c>true</c>
        /// each output turns on and off according to the GS data every 65536 GS reference clocks.
        /// </remarks>
        bool DisplayRepeatMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the display timing reset mode is enabled.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, the GS counter is reset to '0' and all constant-current
        /// outputs are forced off when the internal latch pulse is generated for data latching.
        /// This function is the same when <see cref="Blank"/> is set to <c>false</c>.
        /// Therefore, <see cref="Blank"/> does not need to be controlled by an external controller
        /// when this mode is enabled. If <c>false</c>, the GS counter is not reset and no output
        /// is forced off even if the internal latch pulse is generated.
        /// </remarks>
        bool DisplayTimingResetMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the GS reference clock uses the SCKI clock.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, PWM timing refers to the SCKI clock. If <c>false</c>, PWM timing
        /// refers to the internal oscillator clock.
        /// </remarks>
        bool ReferenceClock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the GS reference clock edge for OUTXn on-off timing control uses rising edge).
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, OUTXn are turned on or off at the rising edge of the selected GS reference clock.
        /// If <c>false</c>, OUTXn are turned on or off at the falling edge of the selected clock.
        /// </remarks>
        bool ReferenceClockEdge { get; set; }

        /// <summary>
        /// Gets or sets the global brightness control for OUTR0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current.
        /// </remarks>
        byte BrightnessControlR { get; set; }

        /// <summary>
        /// Gets or sets the global brightness control for OUTG0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current.
        /// </remarks>
        byte BrightnessControlG { get; set; }

        /// <summary>
        /// Gets or sets the global brightness control for OUTB0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current.
        /// </remarks>
        byte BrightnessControlB { get; set; }
    }
}