using Microsoft.Extensions.DependencyInjection;
using Radical.Validation;
using Radical.Windows.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Radical.Windows.Bootstrap
{
    /// <summary>
    /// Application bootstrap configuration options.
    /// </summary>
    public class BootstrapConfiguration
    {
        internal BootstrapConfiguration() { }

        bool isInitialized = false;
        readonly ResourcesRegistrationHolder resourcesRegistrationHolder = new ResourcesRegistrationHolder();

        internal Func<IServiceProvider, CultureInfo> CultureConfig { get; private set; } = _ => CultureInfo.CurrentCulture;
        internal Func<IServiceProvider, CultureInfo> UICultureConfig { get; private set; } = _ => CultureInfo.CurrentUICulture;
        internal Type ShellViewType { get; private set; }
        internal ShutdownMode? ShutdownMode { get; private set; }
        internal string SingletonRegistrationKey { get; private set; }
        internal SingletonApplicationScope SingletonApplicationScope { get; private set; } = SingletonApplicationScope.NotSupported;
        internal Action<SingletonApplicationStartupArgs> OnSingletonApplicationStartupHandler { get; private set; } = _ => { };
        internal Action<ApplicationShuttingDownArgs> ShuttingDownHandler { get; private set; } = _ => { };
        internal Action<IServiceProvider> BootingHandler { get; private set; } = _ => { };
        internal bool IsAutoBootEnabled { get; private set; } = true;
        internal bool IsSplashScreenEnabled { get; private set; } = false;
        internal SplashScreenConfiguration SplashScreenConfiguration { get; private set; } = new SplashScreenConfiguration();
        internal Action<IServiceProvider> OnServiceProviderCreatedHandler { get; private set; } = _ => { };
        internal Action<Exception> UnhandledExceptionHandler { get; private set; } = _ => { };
        internal Action<IServiceProvider> BootCompletedHandler { get; private set; } = _ => { };

        internal Action<IServiceCollection> ConfigureServicesHandler{ get; private set; } = _ => { };

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

        internal void ExposeRegisteredAppResources(IServiceProvider serviceProvider, Application application)
        {
            if (resourcesRegistrationHolder.Registrations.TryGetValue(application.GetType(), out HashSet<Type> services) && services.Any())
            {
                var conventions = serviceProvider.GetService<IConventionsHandler>();
                foreach (var type in services)
                {
                    var instance = serviceProvider.GetService(type);
                    var key = conventions.GenerateServiceStaticResourceKey(type);
                    application.Resources.Add(key, instance);
                }
            }
        }

        /// <summary>
        /// Customize the services collection before the service provider is created.
        /// </summary>
        /// <param name="services">A callback invoked to customize the services collection.</param>
        public void ConfigureServices(Action<IServiceCollection> services)
        {
            Ensure.That(services).Named(nameof(services)).IsNotNull();
            ConfigureServicesHandler = services;
        }

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
        /// <typeparam name="TService">The type of the service.</typeparam>
        public void ExposeServiceAsResource<TService>()
        {
            ExposeServiceAsResource(typeof(TService), Application.Current.GetType());
        }

        /// <summary>
        /// Exposes the given service type as resource in the supplied resource owner.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TView">The type of the view.</typeparam>
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
        /// <param name="cultureConfig">A delegate that, given the service provider, returns the culture to use.</param>
        public void UseCulture(Func<IServiceProvider, CultureInfo> cultureConfig)
        {
            CultureConfig = cultureConfig;
        }

        /// <summary>
        /// Defines the UICulture to use.
        /// </summary>
        /// <param name="uiCultureConfig">A delegate that, given the service provider, returns the UI culture to use.</param>
        public void UseUICulture(Func<IServiceProvider, CultureInfo> uiCultureConfig)
        {
            UICultureConfig = uiCultureConfig;
        }

        /// <summary>
        /// Defines the type to use as main/shell window.
        /// </summary>
        /// <typeparam name="TShellType">The type of the shell window.</typeparam>
        public void UseAsShell<TShellType>() where TShellType : Window
        {
            UseAsShell(typeof(TShellType));
        }

        /// <summary>
        /// Defines the type to use as main/shell window.
        /// </summary>
        /// <param name="shellViewType">The type of the shell window.</param>
        public void UseAsShell(Type shellViewType)
        {
            Ensure.That(shellViewType)
                .Named(nameof(shellViewType))
                .WithMessage("Only Window is supported as shell type.")
                .Is<Window>();

            ShellViewType = shellViewType;
        }

        /// <summary>
        /// Override the default shutdown mode.
        /// </summary>
        /// <param name="shutdownMode">The shutdown mode to use.</param>
        public void OverrideShutdownMode(ShutdownMode shutdownMode)
        {
            ShutdownMode = shutdownMode;
        }

        /// <summary>
        /// Registers this application as singleton locally at the user session level.
        /// </summary>
        /// <param name="registrationKey">The key that identifies the application.</param>
        public void RegisterAsLocalSingleton(string registrationKey)
        {
            SingletonRegistrationKey = registrationKey;
            SingletonApplicationScope = SingletonApplicationScope.Local;
        }

        /// <summary>
        /// Registers this application as singleton globally at the machine level.
        /// </summary>
        /// <param name="registrationKey">The key that identifies the application.</param>
        public void RegisterAsGlobalSingleton(string registrationKey)
        {
            SingletonRegistrationKey = registrationKey;
            SingletonApplicationScope = SingletonApplicationScope.Global;
        }

        /// <summary>
        /// Called when a singleton application startup.
        /// </summary>
        /// <param name="onSingletonApplicationStartup">The handler invoked when the singleton application starts.</param>
        public void OnSingletonApplicationStartup(Action<SingletonApplicationStartupArgs> onSingletonApplicationStartup)
        {
            Ensure.That(onSingletonApplicationStartup)
                .Named(nameof(onSingletonApplicationStartup))
                .IsNotNull();

            OnSingletonApplicationStartupHandler = onSingletonApplicationStartup;
        }

        /// <summary>
        /// Called when the application is booting.
        /// </summary>
        /// <param name="bootingHandler">The handler invoked when the application is booting.</param>
        public void OnBooting(Action<IServiceProvider> bootingHandler)
        {
            Ensure.That(bootingHandler)
                .Named(nameof(bootingHandler))
                .IsNotNull();

            BootingHandler = bootingHandler;
        }

        /// <summary>
        /// Called when the application is shutting down.
        /// </summary>
        /// <param name="shutingDownHandler">The handler invoked when the application is shutting down.</param>
        public void OnShuttingDown(Action<ApplicationShuttingDownArgs> shutingDownHandler)
        {
            Ensure.That(shutingDownHandler)
                .Named(nameof(shutingDownHandler))
                .IsNotNull();

            ShuttingDownHandler = shutingDownHandler;
        }

        /// <summary>
        /// Disables the application to automatically boot when bound to a WPF Application.
        /// </summary>
        public void DisableAutoBoot()
        {
            IsAutoBootEnabled = false;
        }

        /// <summary>
        /// Enables splash screen support.
        /// </summary>
        /// <param name="config">The splash screen configuration, or <c>null</c> to use the default one.</param>
        public void EnableSplashScreen(SplashScreenConfiguration config = null)
        {
            if (config != null)
            {
                SplashScreenConfiguration = config;
            }

            IsSplashScreenEnabled = true;
        }

        /// <summary>
        /// Called when the service provider is created.
        /// </summary>
        /// <param name="onServiceProviderCreatedHandler">The handler invoked when the service provider is created.</param>
        public void OnServiceProviderCreated(Action<IServiceProvider> onServiceProviderCreatedHandler)
        {
            Ensure.That(onServiceProviderCreatedHandler)
                .Named(nameof(onServiceProviderCreatedHandler))
                .IsNotNull();

            OnServiceProviderCreatedHandler = onServiceProviderCreatedHandler;
        }

        /// <summary>
        /// Allows to inject an handler for not handled exception(s).
        /// </summary>
        /// <param name="unhandledExceptionHandler">The handler invoked when an unhandled exception occurs.</param>
        public void OnUnhandledException(Action<Exception> unhandledExceptionHandler)
        {
            Ensure.That(unhandledExceptionHandler)
                .Named(nameof(unhandledExceptionHandler))
                .IsNotNull();

            UnhandledExceptionHandler = unhandledExceptionHandler;
        }

        /// <summary>
        /// Called when the boot process is completed.
        /// </summary>
        /// <param name="bootCompletedHandler">The handler invoked when the boot process is completed.</param>
        public void OnBootCompleted(Action<IServiceProvider> bootCompletedHandler)
        {
            Ensure.That(bootCompletedHandler)
                .Named(nameof(bootCompletedHandler))
                .IsNotNull();

            BootCompletedHandler = bootCompletedHandler;
        }
    }
}