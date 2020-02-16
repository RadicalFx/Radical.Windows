using Microsoft.Extensions.DependencyInjection;
using Radical.Validation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

//TODO: change namespace and class name?
namespace Radical.Windows.Bootstrap
{
    /// <summary>
    /// Application bootstrap configuration options.
    /// </summary>
    public class BootstrapConfiguration
    {
        bool isInitialized = false;
        readonly ResourcesRegistrationHolder resourcesRegistrationHolder = new ResourcesRegistrationHolder();

        internal Func<IServiceProvider, CultureInfo> CultureConfig { get; private set; } = _ => CultureInfo.CurrentCulture;
        internal Func<IServiceProvider, CultureInfo> UICultureConfig { get; private set; } = _ => CultureInfo.CurrentUICulture;
        internal Type ShellViewType { get; private set; }

        /// <summary>
        /// The set of conventions used to bootstrap
        /// the application and configure DI.
        /// </summary>
        public BootstrapConventions BootstrapConventions { get; } = new BootstrapConventions();

        /// <summary>
        /// The assembly scanner used to scan for assembles and types
        /// to include in the DI configuration step.
        /// </summary>
        public AssemblyScanner AssemblyScanner { get; } = new AssemblyScanner();

        /// <summary>
        /// Exposes the given service type as resource in the App resources.
        /// </summary>
        public void ExposeServiceAsResource<TService>()
        {
            ExposeServiceAsResource(typeof(TService), Application.Current.GetType());
        }

        /// <summary>
        /// Exposes the given service type as resource in the supplied resource owner.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <returns></returns>
        public void ExposeServiceAsResource<TService, TView>() where TView : FrameworkElement
        {
            ExposeServiceAsResource(typeof(TService), typeof(TView));
        }

        internal void ExposeServiceAsResource(Type serviceType, Type resourceOwner)
        {
            if (!resourcesRegistrationHolder.Registrations.TryGetValue(resourceOwner, out HashSet<Type> types))
            {
                types = new HashSet<Type>();
                resourcesRegistrationHolder.Registrations.Add(resourceOwner, new HashSet<Type>());
            }

            Ensure.That(types)
                .WithMessage("Supplied service type ({0}) is already exposed as resource in {1}.", serviceType.Name, resourceOwner.Name)
                .IsFalse(hs => hs.Contains(serviceType));

            types.Add(serviceType);
        }

        /// <summary>
        /// Defines the Culture to use.
        /// </summary>
        public void UseCulture(Func<IServiceProvider, CultureInfo> cultureConfig)
        {
            CultureConfig = cultureConfig;
        }

        /// <summary>
        /// Defines the UICulture to use.
        /// </summary>
        public void UseUICulture(Func<IServiceProvider, CultureInfo> uiCultureConfig)
        {
            UICultureConfig = uiCultureConfig;
        }

        internal void PopulateServiceCollection(IServiceCollection services)
        {
            Ensure.That(this).IsFalse(cfg => cfg.isInitialized);

            services.AddSingleton(this);
            services.AddSingleton(BootstrapConventions);
            services.AddSingleton(resourcesRegistrationHolder);

            var assemblies = AssemblyScanner.Scan();
            var allTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Distinct()
                .ToArray();

            foreach (var installerType in allTypes.Where(t => typeof(IDependenciesInstaller).IsAssignableFrom(t) && !t.IsAbstract))
            {
                var installer = (IDependenciesInstaller)Activator.CreateInstance(installerType);
                installer.Install(BootstrapConventions, services, allTypes);
            }

            isInitialized = true;
        }

        /// <summary>
        /// Defines the type to use as main/shell window.
        /// </summary>
        public void UseAsShell<TShellType>() where TShellType : Window
        {
            UseAsShell(typeof(TShellType));
        }

        /// <summary>
        /// Defines the type to use as main/shell window.
        /// </summary>
        public void UseAsShell(Type shellViewType)
        {
            Ensure.That(shellViewType)
                .Named(nameof(shellViewType))
                .WithMessage("Only Window is supported as shell type.")
                .Is<Window>();

            ShellViewType = shellViewType;
        }
    }
}
