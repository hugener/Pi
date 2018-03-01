// <copyright file="ByteExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Controllers.Tlc59711
{
    using global::System;

    internal static class ByteExtensionMethods
    {
        public const byte BrightnessControlMax = 127;

        public static void ThrowOnInvalidBrightnessControl(this byte value)
        {
            if (value <= BrightnessControlMax)
            {
                return;
            }

            var message = string.Format("The maximum value for brightness control is {0}. You set a value of {1}.", BrightnessControlMax, value);

            throw new ArgumentException(message, "value");
        }
    }
}