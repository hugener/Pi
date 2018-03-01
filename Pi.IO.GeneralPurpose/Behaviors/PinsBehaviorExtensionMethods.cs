// <copyright file="PinsBehaviorExtensionMethods.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.GeneralPurpose.Behaviors
{
    /// <summary>
    /// Provides extension methods for <see cref="PinsBehavior"/>.
    /// </summary>
    public static class PinsBehaviorExtensionMethods
    {
        /// <summary>
        /// Starts the specified behavior on the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="behavior">The behavior.</param>
        public static void Start(this GpioConnection connection, PinsBehavior behavior)
        {
            foreach (var configuration in behavior.Configurations)
            {
                if (!connection.Contains(configuration))
                {
                    connection.Add(configuration);
                }
            }

            behavior.Start(connection);
        }

        /// <summary>
        /// Stops the specified behavior.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="behavior">The behavior.</param>
        public static void Stop(this GpioConnection connection, PinsBehavior behavior)
        {
            behavior.Stop();
        }
    }
}