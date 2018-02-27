namespace Pi.Timers
{
    /// <summary>
    /// Provides access to timing features.
    /// </summary>
    public static class Timer
    {
        /// <summary>
        /// Creates a timer.
        /// </summary>
        /// <returns>The timer.</returns>
        /// <remarks>
        /// The created timer is the most suitable for the current platform.
        /// </remarks>
        public static ITimer Create()
        {
            return Board.Current.IsRaspberryPi
                       ? (ITimer) new HighResolutionTimer()
                       : new StandardTimer();
        }

        /// <summary>
        /// Disposes the specified timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        public static void Dispose(ITimer timer)
        {
            timer.Dispose();
        }
    }
}