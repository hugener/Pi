// <copyright file="Units.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Sensors.Distance.HcSr04
{
    using global::System;
    using UnitsNet;

    internal static class Units
    {
        /// <summary>
        /// Velocity related conversions
        /// </summary>
        public static class Velocity
        {
            /// <summary>
            /// Sound velocity related conversions
            /// </summary>
            public static class Sound
            {
                /// <summary>
                /// Converts a time to a distance.
                /// </summary>
                /// <param name="time">The time.</param>
                /// <returns>The distance travelled by sound in one second, in meters.</returns>
                public static Length ToDistance(TimeSpan time)
                {
                    if (time < TimeSpan.Zero)
                    {
                        return Length.FromMeters(double.MinValue);
                    }

                    return Length.FromMeters(time.TotalMilliseconds * 340 / 1000);
                }
            }
        }
    }
}