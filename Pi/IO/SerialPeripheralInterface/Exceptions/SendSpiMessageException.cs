// <copyright file="SendSpiMessageException.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;
    using global::System.Runtime.Serialization;

    /// <summary>
    /// Exception for sending a spi message.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class SendSpiMessageException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSpiMessageException"/> class.
        /// </summary>
        public SendSpiMessageException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendSpiMessageException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SendSpiMessageException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendSpiMessageException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public SendSpiMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendSpiMessageException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected SendSpiMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}