using Radical.Windows;
using Radical.Windows.Bootstrap;

namespace System.Windows
{
    /// <summary>
    /// Extension methods to bootstrap a <see cref="RadicalApplication"/> from a WPF <see cref="Application"/>.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Add a RadicalApplication to the current WPF application
        /// </summary>
        /// <param name="application">The WPF application to bind the Radical application to.</param>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <returns>The configured <see cref="RadicalApplication"/>.</returns>
        public static RadicalApplication AddRadicalApplication(this Application application, Action<BootstrapConfiguration> configure)
        {
            return AddRadicalApplication(application, null, configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current WPF application
        /// </summary>
        /// <typeparam name="TShellView">The type of the shell window.</typeparam>
        /// <param name="application">The WPF application to bind the Radical application to.</param>
        /// <returns>The configured <see cref="RadicalApplication"/>.</returns>
        public static RadicalApplication AddRadicalApplication<TShellView>(this Application application) where TShellView : Window
        {
            return AddRadicalApplication(application, typeof(TShellView), null);
        }

        /// <summary>
        /// Add a RadicalApplication to the current WPF application
        /// </summary>
        /// <typeparam name="TShellView">The type of the shell window.</typeparam>
        /// <param name="application">The WPF application to bind the Radical application to.</param>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <returns>The configured <see cref="RadicalApplication"/>.</returns>
        public static RadicalApplication AddRadicalApplication<TShellView>(this Application application, Action<BootstrapConfiguration> configure) where TShellView : Window
        {
            return AddRadicalApplication(application, typeof(TShellView), configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current WPF application
        /// </summary>
        /// <param name="application">The WPF application to bind the Radical application to.</param>
        /// <param name="shellViewType">The type of the shell window, or <c>null</c> to use the default one.</param>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <returns>The configured <see cref="RadicalApplication"/>.</returns>
        public static RadicalApplication AddRadicalApplication(this Application application, Type shellViewType, Action<BootstrapConfiguration> configure)
        {
            var configuration = new BootstrapConfiguration();
            if (shellViewType != null)
            {
                configuration.UseAsShell(shellViewType);
            }
            configure?.Invoke(configuration);

            return RadicalApplication.BoundTo(application, configuration);
        }
    }
}