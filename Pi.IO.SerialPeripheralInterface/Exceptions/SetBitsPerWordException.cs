// <copyright file="SetBitsPerWordException.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.IO.SerialPeripheralInterface
{
    using global::System;
    using global::System.Runtime.Serialization;

#pragma warning disable 1591
    [Serializable]
    public class SetBitsPerWordException : Exception
    {
        public SetBitsPerWordException()
        {
        }

        public SetBitsPerWordException(string message)
            : base(message)
        {
        }

        public SetBitsPerWordException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SetBitsPerWordException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
#pragma warning restore 1591
}