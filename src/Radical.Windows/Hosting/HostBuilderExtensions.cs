using Microsoft.Extensions.DependencyInjection;
using Radical.Validation;
using Radical.Windows;
using Radical.Windows.Bootstrap;
using Radical.Windows.Hosting;
using System;
using System.Windows;

namespace Microsoft.Extensions.Hosting
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
        public static IHostBuilder AddRadicalApplication(this IHostBuilder hostBuilder, Action<BootstrapConfiguration> configure)
        {
            return AddRadicalApplication(hostBuilder, null, configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
        public static IHostBuilder AddRadicalApplication<TShellView>(this IHostBuilder hostBuilder, Action<BootstrapConfiguration> configure) where TShellView: Window
        {
            return AddRadicalApplication(hostBuilder, typeof(TShellView), configure);
        }

        /// <summary>
        /// Add a RadicalApplication to the current host builder
        /// </summary>
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

            Ensure.That(configure)
                .Named(nameof(configure))
                .IsNotNull();

            configure(configuration);

            hostBuilder.ConfigureServices((context,serviceCollection) =>
            {
                configuration.PopulateServiceCollection(serviceCollection);

                serviceCollection.AddHostedService<RadicalApplicationService>();
            });

            hostBuilder.Properties.Add("RadicalApplicationAdded", new object());

            return hostBuilder;
        }
    }
}
