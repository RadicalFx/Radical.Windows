using Microsoft.Extensions.DependencyInjection;
using Radical.Windows.Bootstrap;
using Radical.Windows.Hosting;
using System;
using System.Windows;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extension methods to add a Radical application to an <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <returns>The host builder, for chaining.</returns>
        public static IHostBuilder AddRadicalApplication(this IHostBuilder hostBuilder, Action<BootstrapConfiguration> configure)
        {
            return AddRadicalApplication(hostBuilder, null, configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
        /// <typeparam name="TShellView">The type of the shell window.</typeparam>
        /// <param name="hostBuilder">The host builder.</param>
        /// <returns>The host builder, for chaining.</returns>
        public static IHostBuilder AddRadicalApplication<TShellView>(this IHostBuilder hostBuilder) where TShellView : Window
        {
            return AddRadicalApplication(hostBuilder, typeof(TShellView), null);
        }

        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
        /// <typeparam name="TShellView">The type of the shell window.</typeparam>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <returns>The host builder, for chaining.</returns>
        public static IHostBuilder AddRadicalApplication<TShellView>(this IHostBuilder hostBuilder, Action<BootstrapConfiguration> configure) where TShellView : Window
        {
            return AddRadicalApplication(hostBuilder, typeof(TShellView), configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
        /// <param name="configure">A delegate to customize the bootstrap configuration, or <c>null</c>.</param>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="shellType">The type of the shell window, or <c>null</c> to use the default one.</param>
        /// <returns>The host builder, for chaining.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        public static IHostBuilder AddRadicalApplication(this IHostBuilder hostBuilder, Type shellType, Action<BootstrapConfiguration> configure)
        {
            if (hostBuilder.Properties.TryGetValue("RadicalApplicationAdded", out object val))
            {
                throw new InvalidOperationException("A Radical application has been already added to the current host.");
            }

            var configuration = new BootstrapConfiguration();
            if (shellType != null)
            {
                configuration.UseAsShell(shellType);
            }

            configure?.Invoke(configuration);

            hostBuilder.ConfigureServices((context, serviceCollection) =>
            {
                configuration.ConfigureServicesHandler(serviceCollection);
                configuration.PopulateServiceCollection(serviceCollection);

                serviceCollection.AddHostedService<RadicalApplicationService>();
            });

            hostBuilder.Properties.Add("RadicalApplicationAdded", new object());

            return hostBuilder;
        }
    }
}