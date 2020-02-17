using Radical.Windows;
using Radical.Windows.Bootstrap;

namespace System.Windows
{
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Add a RadicalApplication to the current WPF application
        /// </summary>
        public static RadicalApplication AddRadicalApplication(this Application application, Action<BootstrapConfiguration> configure)
        {
            return AddRadicalApplication(application, null, configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current WPF application
        /// </summary>
        public static RadicalApplication AddRadicalApplication<TShellView>(this Application application) where TShellView : Window
        {
            return AddRadicalApplication(application, typeof(TShellView), null);
        }

        /// <summary>
        /// Add a RadicalApplication to the current WPF application
        /// </summary>
        public static RadicalApplication AddRadicalApplication<TShellView>(this Application application, Action<BootstrapConfiguration> configure) where TShellView : Window
        {
            return AddRadicalApplication(application, typeof(TShellView), configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current WPF application
        /// </summary>
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