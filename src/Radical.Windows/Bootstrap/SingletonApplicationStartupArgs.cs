namespace Radical.Windows.Bootstrap
{
    /// <summary>
    /// Arguments to handle the application startup when singleton mode is set up.
    /// </summary>
    public class SingletonApplicationStartupArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonApplicationStartupArgs"/> class.
        /// </summary>
        public SingletonApplicationStartupArgs(SingletonApplicationScope scope, string singletonRegistrationKey)
        {
            Scope = scope;
            AllowStartup = true;
            SingletonRegistrationKey = singletonRegistrationKey;
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public SingletonApplicationScope Scope { get; }

        /// <summary>
        /// Get the registration key
        /// </summary>
        public string SingletonRegistrationKey { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the startup is allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the startup is allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowStartup { get; set; }
    }
}
