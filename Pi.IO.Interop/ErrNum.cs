using System;
using System.Runtime.InteropServices;

namespace Pi.IO.Interop
{
    /// <summary>
    /// Helper methods for P/Invoke errors.
    /// </summary>
    public static class ErrNum
    {
        #region libc imports
        [DllImport("libc", EntryPoint = "strerror", SetLastError = true)]
        private static extern IntPtr strerror(int errnum);
        #endregion

        #region Methods
        /// <summary>
        /// Throws the on p invoke error.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="Exception">The exception type.</exception>
        public static void ThrowOnPInvokeError<TException>(this int result, string message = null)
            where TException : Exception, new() 
        {
            if (result >= 0) {
                return;
            }

            var type = typeof(TException);
            var constructorInfo = type.GetConstructor(new[] { typeof(string) });
            if (ReferenceEquals(constructorInfo, null)) {
                throw new TException();
            }
            
            var err = Marshal.GetLastWin32Error();
            var messagePtr = strerror(err);

            var strErrorMessage = (messagePtr != IntPtr.Zero)
                ? Marshal.PtrToStringAuto(messagePtr)
                : "unknown";

            var exceptionMessage = (message == null)
                ? string.Format("Error {0}: {1}", err, strErrorMessage)
                : string.Format(message, result, err, strErrorMessage);
            
            throw (TException)constructorInfo.Invoke(new object[] { exceptionMessage });
        }
        #endregion
    }
}