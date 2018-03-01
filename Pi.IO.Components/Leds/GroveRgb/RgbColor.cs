// <copyright file="RgbColor.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Leds.GroveRgb
{
    using global::System;

    /// <summary>
    /// Represents a RGB color in 3 bytes.
    /// </summary>
    public class RgbColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RgbColor"/> class.
        /// </summary>
        public RgbColor()
        {
            this.Red = 0;
            this.Green = 0;
            this.Blue = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RgbColor"/> class.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        public RgbColor(byte r, byte g, byte b)
        {
            this.Red = r;
            this.Green = g;
            this.Blue = b;
        }

        /// <summary>
        /// Gets or sets the red.
        /// </summary>
        /// <value>
        /// The red.
        /// </value>
        public byte Red { get; set; }

        /// <summary>
        /// Gets or sets the green.
        /// </summary>
        /// <value>
        /// The green.
        /// </value>
        public byte Green { get; set; }

        /// <summary>
        /// Gets or sets the blue.
        /// </summary>
        /// <value>
        /// The blue.
        /// </value>
        public byte Blue { get; set; }

        /// <summary>
        /// Gets RgbColor instance from Hsv color space.
        /// </summary>
        /// <param name="hue">Hue.</param>
        /// <param name="sat">Saturation.</param>
        /// <param name="val">Value.</param>
        /// <returns>A <see cref="RgbColor"/>.</returns>
        public static RgbColor FromHsv(double hue, double sat, double val)
        {
            byte r = 0, g = 0, b = 0;
            double hValue = hue * 360D;
            while (hValue < 0)
            {
                hValue += 360;
            }

            while (hValue >= 360)
            {
                hValue -= 360;
            }

            double rValue, gValue, bValue;
            if (val <= 0)
            {
                rValue = gValue = bValue = 0;
            }
            else if (sat <= 0)
            {
                rValue = gValue = bValue = val;
            }
            else
            {
                double hf = hValue / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = val * (1 - sat);
                double qv = val * (1 - (sat * f));
                double tv = val * (1 - (sat * (1 - f)));
                switch (i)
                {
                    case 0: // Red is the dominant color
                        rValue = val;
                        gValue = tv;
                        bValue = pv;
                        break;
                    case 1: // Green is the dominant color
                        rValue = qv;
                        gValue = val;
                        bValue = pv;
                        break;
                    case 2:
                        rValue = pv;
                        gValue = val;
                        bValue = tv;
                        break;
                    case 3: // Blue is the dominant color
                        rValue = pv;
                        gValue = qv;
                        bValue = val;
                        break;
                    case 4:
                        rValue = tv;
                        gValue = pv;
                        bValue = val;
                        break;
                    case 5: // Red is the dominant color
                        rValue = val;
                        gValue = pv;
                        bValue = qv;
                        break;
                    case 6: // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                        rValue = val;
                        gValue = tv;
                        bValue = pv;
                        break;
                    case -1:
                        rValue = val;
                        gValue = pv;
                        bValue = qv;
                        break;
                    default: // The color is not defined, we should throw an error.
                        rValue = gValue = bValue = val; // Just pretend its black/white
                        break;
                }
            }

            r = (byte)Clamp((int)(rValue * 255.0));
            g = (byte)Clamp((int)(gValue * 255.0));
            b = (byte)Clamp((int)(bValue * 255.0));

            return new RgbColor { Red = r, Green = g, Blue = b };
        }

        private static int Clamp(int i)
        {
            if (i < 0)
            {
                i = 0;
            }

            if (i > 255)
            {
                i = 255;
            }

            return i;
        }
    }
}