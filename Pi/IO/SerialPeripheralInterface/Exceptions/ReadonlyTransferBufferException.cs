// <copyright file="ReadonlyTransferBufferException.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;
    using global::System.Runtime.Serialization;

#pragma warning disable 1591
    [Serializable]
    public class ReadOnlyTransferBufferException : Exception
    {
        public ReadOnlyTransferBufferException()
        {
        }

        public ReadOnlyTransferBufferException(string message)
            : base(message)
        {
        }

        public ReadOnlyTransferBufferException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ReadOnlyTransferBufferException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
#pragma warning restore 1591
}