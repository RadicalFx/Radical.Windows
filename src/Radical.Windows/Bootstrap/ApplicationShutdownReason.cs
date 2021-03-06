﻿namespace Radical.Windows.Bootstrap
{
    /// <summary>
    /// Determines the reason for application shutdown.
    /// </summary>
    public enum ApplicationShutdownReason
    {
        /// <summary>
        /// The application has been shutdown using the Radical canonical behaviors.
        /// In this case the shutdown process can be canceled.
        /// </summary>
        UserRequest = 0,

        /// <summary>
        /// The application is shutting down because another 
        /// instance is already running and the application 
        /// is marked as singleton.
        /// </summary>
        MultipleInstanceNotAllowed = 1,

        /// <summary>
        /// The application is shutting down because the operating system session is ending.
        /// </summary>
        SessionEnding,

        /// <summary>
        /// The application has been shut down using the App.Current.Shutdown() method.
        /// </summary>
        ApplicationRequest,
    }
}
