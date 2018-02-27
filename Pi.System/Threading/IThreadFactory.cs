namespace Pi.System.Threading
{
    /// <summary>
    /// Interface for implementing a thread factory.
    /// </summary>
    public interface IThreadFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>An <see cref="IThread"/>.</returns>
        IThread Create();
    }
}