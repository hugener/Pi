// <copyright file="WriteOnlyTransferBufferException.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;
    using global::System.Runtime.Serialization;

#pragma warning disable 1591
    [Serializable]
    public class WriteOnlyTransferBufferException : Exception
    {
        public WriteOnlyTransferBufferException()
        {
        }

        public WriteOnlyTransferBufferException(string message)
            : base(message)
        {
        }

        public WriteOnlyTransferBufferException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected WriteOnlyTransferBufferException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
#pragma warning restore 1591
}