// <copyright file="ProcessorPinExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;

    /// <summary>
    /// Provides extension methods for <see cref="ProcessorPin"/> and <see cref="ProcessorPins"/> objects.
    /// </summary>
    public static class ProcessorPinExtensionMethods
    {
        /// <summary>
        /// Enumerates the specified pins.
        /// </summary>
        /// <param name="pins">The pins.</param>
        /// <returns>The pins as an enumerable.</returns>
        public static IEnumerable<ProcessorPin> Enumerate(this ProcessorPins pins)
        {
            return (Enum.GetValues(typeof(ProcessorPin)) as ProcessorPin[] ?? new ProcessorPin[0])
                .Distinct()
                .Where(p => (pins & (ProcessorPins)((uint)1 << (int)p)) != ProcessorPins.None)
                .ToArray();
        }
    }
}