using Microsoft.Extensions.DependencyInjection;
using Radical.Validation;
using System;
using System.Globalization;
using System.Windows;

//TODO: change namespace and class name?
namespace Radical.Windows.Bootstrap
{
    /// <summary>
    /// Application bootstrap configuration options.
    /// </summary>
    public class BootstrapConfiguration
    {
        internal Func<IServiceProvider, BootstrapConventions, CultureInfo> CultureConfig { get; private set; } = (_, __) => CultureInfo.CurrentCulture;
        internal Func<IServiceProvider, BootstrapConventions, CultureInfo> UICultureConfig { get; private set; } = (_, __) => CultureInfo.CurrentUICulture;
        internal Type ShellViewType { get; private set; }

        /// <summary>
        /// Defines the Culture to use.
        /// </summary>
        public void UseCulture(Func<IServiceProvider, BootstrapConventions, CultureInfo> cultureConfig)
        {
            CultureConfig = cultureConfig;
        }

        /// <summary>
        /// Defines the UICulture to use.
        /// </summary>
        public void UseUICulture(Func<IServiceProvider, BootstrapConventions, CultureInfo> uiCultureConfig)
        {
            UICultureConfig = uiCultureConfig;
        }

        /// <summary>
        /// Use an externally provided ServiceCollection
        /// </summary>
        public void UseServiceCollection(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the type to use as main/shell window.
        /// </summary>
        public void UseShell<TShellType>() where TShellType : Window
        {
            UseShell(typeof(TShellType));
        }

        /// <summary>
        /// Defines the type to use as main/shell window.
        /// </summary>
        public void UseShell(Type shellViewType)
        {
            Ensure.That(shellViewType)
                .WithMessage("Only Window is supported as shell type.")
                .Is<Window>();

            ShellViewType = shellViewType;
        }
    }
}
