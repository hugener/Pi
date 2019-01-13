// <copyright file="IThreadFactory.cs" company="Pi">
// Copyright (c) Pi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Pi.System.Threading
{
    using Sundew.Base.Threading;

    /// <summary>
    /// Interface for implementing a thread factory.
    /// </summary>
    public interface IThreadFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>An <see cref="ICurrentThread"/>.</returns>
        ICurrentThread Create();
    }
}