using System;

namespace Radical.Windows
{
    /// <summary>
    /// An <see cref="ApplicationBootstrapper"/> that uses <typeparamref name="TMainView"/> as the shell window.
    /// </summary>
    /// <typeparam name="TMainView">The type of the shell window.</typeparam>
    [Obsolete("ApplicationBootstrapper has been obsoleted and will be removed in v5.0.0. Use RadicalApplication, through the AddRadicalApplication extension method, instead.", true)]
    public class ApplicationBootstrapper<TMainView> : ApplicationBootstrapper where TMainView : System.Windows.Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBootstrapper{TMainView}"/> class.
        /// </summary>
        public ApplicationBootstrapper()
            : base()
        {
            UsingAsShell<TMainView>();
        }
    }
}
