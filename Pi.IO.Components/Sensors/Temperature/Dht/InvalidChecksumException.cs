// <copyright file="InvalidChecksumException.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.Components.Sensors.Temperature.Dht
{
    using global::System;
    using global::System.Globalization;

    /// <summary>
    /// Exception for an invalid checksum.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidChecksumException : Exception
    {
        private readonly long expectedValue;
        private readonly long actualValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidChecksumException"/> class.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        public InvalidChecksumException(long expectedValue, long actualValue)
            : base(GetMessage(null, expectedValue, actualValue))
        {
            this.expectedValue = expectedValue;
            this.actualValue = actualValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidChecksumException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        public InvalidChecksumException(string message, long expectedValue, long actualValue)
            : base(GetMessage(message, expectedValue, actualValue))
        {
            this.expectedValue = expectedValue;
            this.actualValue = actualValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidChecksumException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        public InvalidChecksumException(string message, Exception innerException, long expectedValue, long actualValue)
            : base(GetMessage(message, expectedValue, actualValue), innerException)
        {
            this.expectedValue = expectedValue;
            this.actualValue = actualValue;
        }

        /// <summary>
        /// Gets the expected value.
        /// </summary>
        /// <value>
        /// The expected value.
        /// </value>
        public long ExpectedValue => this.expectedValue;

        /// <summary>
        /// Gets the actual value.
        /// </summary>
        /// <value>
        /// The actual value.
        /// </value>
        public long ActualValue => this.actualValue;

        private static string GetMessage(string message, long expectedValue, long actualValue)
        {
            var valueMessage = string.Format(CultureInfo.InvariantCulture, "Expected {0}, found {1}", expectedValue, actualValue);
            return !string.IsNullOrEmpty(message)
                ? message + ". " + valueMessage
                : valueMessage;
        }
    }
}