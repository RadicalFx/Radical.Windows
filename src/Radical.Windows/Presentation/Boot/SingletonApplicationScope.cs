namespace Radical.Windows.Presentation.Boot
{
    /// <summary>
    /// Determines the scope of a singleton application.
    /// </summary>
    public enum SingletonApplicationScope
    {
        /// <summary>
        /// Singleton application is not supported.
        /// </summary>
        NotSupported = 0,

        /// <summary>
        /// The application is required to be singleton across sessions.
        /// </summary>
        Global = 1,

        /// <summary>
        /// The application is required to be singleton only for the current session.
        /// </summary>
        Local = 2
    }
}
