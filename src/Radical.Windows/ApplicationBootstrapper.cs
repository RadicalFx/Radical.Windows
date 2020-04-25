using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel.Messaging;
using Radical.Diagnostics;
using Radical.Helpers;
using Radical.Linq;
using Radical.Reflection;
using Radical.Validation;
using Radical.Windows.Bootstrap;
using Radical.Windows.ComponentModel;
using Radical.Windows.Messaging;
using Radical.Windows.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Radical.Windows
{
    /// <summary>
    /// The application bootstrapper. Provides a way to dramatically simplify the
    /// application boot process.
    /// </summary>
    [Obsolete("ApplicationBootstrapper has been obsoleted and will be treated as an error in v3.0.0 and removed in v4.0.0. Consider moving to new RadicalApplication using the AddRadicalApplication extension method.", false)]
    public class ApplicationBootstrapper
    {
        static readonly TraceSource logger = new TraceSource(typeof(ApplicationBootstrapper).Name);

        readonly BootstrapConfiguration bootstrapConfiguration = new BootstrapConfiguration();

        Type shellViewType = null;
        IServiceProvider serviceProvider;

        bool isAutoBootEnabled = true;
        bool isBootCompleted;

        private Action<IServiceProvider> bootCompletedHandler;
        private Action<ApplicationShuttingDownArgs> shutdownHandler;
        private Action<IServiceProvider> bootHandler;
        bool isSessionEnding;
        bool isShuttingDown;
        ShutdownMode? shutdownMode = null;
        Mutex mutex;
        string key;
        SingletonApplicationScope singleton = SingletonApplicationScope.NotSupported;
        readonly AssemblyScanner assemblyScanner = new AssemblyScanner();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBootstrapper"/> class.
        /// </summary>
        public ApplicationBootstrapper()
        {
            var commandLine = CommandLine.GetCurrent();

            if (commandLine.Contains("radical-wait-for-debugger") && !Debugger.IsAttached)
            {
                logger.Warning("Application is waiting for the debugger...");

                int waitCycle = 0;
                while (!Debugger.IsAttached && waitCycle <= 100)
                {
                    Thread.Sleep(600);
                    waitCycle++;
                }

                if (!Debugger.IsAttached)
                {
                    logger.Warning("Waiting for the debugger overlapped the maximum wait time of 1 minute, application will start now.");
                }
            }
            else if (commandLine.Contains("radical-debugger-break") && !Debugger.IsAttached)
            {
                Debugger.Break();
            }

            Application.Current.Startup += (s, e) =>
            {
                if (isAutoBootEnabled)
                {
                    OnBoot();
                }
            };

            Application.Current.SessionEnding += (s, e) =>
            {
                isSessionEnding = true;
            };

            Application.Current.DispatcherUnhandledException += (s, e) =>
            {
                var ex = e.Exception;
                OnUnhandledException(ex);
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                OnUnhandledException(ex);
            };

            Application.Current.Exit += (s, e) =>
            {
                if (!isShuttingDown)
                {
                    var reason = isSessionEnding ? ApplicationShutdownReason.SessionEnding : ApplicationShutdownReason.ApplicationRequest;
                    OnShutdownCore(reason);
                }
            };
        }

        /// <summary>
        /// Defines the type to use as main/shell window.
        /// </summary>
        /// <typeparam name="TShellType">The shell type.</typeparam>
        /// <returns></returns>
        public ApplicationBootstrapper UsingAsShell<TShellType>() where TShellType : Window
        {
            return UsingAsShell(typeof(TShellType));
        }

        /// <summary>
        /// Defines the type to use as main/shell window.
        /// </summary>
        /// <param name="shellViewType">The shell type.</param>
        /// <returns></returns>
        public ApplicationBootstrapper UsingAsShell(Type shellViewType)
        {
            Ensure.That(shellViewType)
                .WithMessage("Only Window is supported as shell type.")
                .Is<Window>();

            this.shellViewType = shellViewType;

            return this;
        }

        private bool isSplashScreenEnabled = false;
        SplashScreenConfiguration splashScreenConfiguration = new SplashScreenConfiguration();

        /// <summary>
        /// Enables splash screen support.
        /// </summary>
        /// <param name="config">The splash screen configuration.</param>
        /// <returns></returns>
        public ApplicationBootstrapper EnableSplashScreen(SplashScreenConfiguration config = null)
        {
            if (config != null)
            {
                splashScreenConfiguration = config;
            }

            isSplashScreenEnabled = true;

            return this;
        }

        /// <summary>
        /// Disables the auto boot.
        /// </summary>
        public ApplicationBootstrapper DisableAutoBoot()
        {
            isAutoBootEnabled = false;

            return this;
        }

        Action<BootstrapConventions, AssemblyScanner> onBeforeInstall;

        /// <summary>
        /// Called before the install and boot process begins, right after the service provider creation.
        /// </summary>
        /// <param name="onBeforeInstall">The on before install.</param>
        /// <returns></returns>
        public ApplicationBootstrapper OnBeforeInstall(Action<BootstrapConventions, AssemblyScanner> onBeforeInstall)
        {
            this.onBeforeInstall = onBeforeInstall;

            return this;
        }

        Action<IServiceProvider> onServiceProviderCreated;

        /// <summary>
        /// Called when the service provider is created.
        /// </summary>
        /// <param name="onServiceProviderCreated">The on service provider created.</param>
        /// <returns></returns>
        public ApplicationBootstrapper OnServiceProviderCreated(Action<IServiceProvider> onServiceProviderCreated)
        {
            this.onServiceProviderCreated = onServiceProviderCreated;

            return this;
        }

        void OnBoot()
        {
            var services = new ServiceCollection();

            var conventions = new BootstrapConventions();
            onBeforeInstall?.Invoke(conventions, assemblyScanner);
            services.AddSingleton(conventions);
            services.AddSingleton(resourcesRegistrationHolder);

            var assemblies = assemblyScanner.Scan();
            var allTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Distinct()
                .ToArray();

            foreach (var installerType in allTypes.Where(t => typeof(IDependenciesInstaller).IsAssignableFrom(t) && !t.IsAbstract))
            {
                var installer = (IDependenciesInstaller)Activator.CreateInstance(installerType);
                installer.Install(conventions, services, allTypes);
            }

            serviceProvider = services.BuildServiceProvider();
            onServiceProviderCreated?.Invoke(serviceProvider);

            var features = serviceProvider.GetServices<IFeature>();
            foreach (var feature in features)
            {
                feature.Setup(serviceProvider, bootstrapConfiguration);
            }

            if (shutdownMode != null && shutdownMode.HasValue)
            {
                Application.Current.ShutdownMode = shutdownMode.Value;
            }

            OnBoot(serviceProvider);

            if (!isShuttingDown)
            {
                OnBootCompleted(serviceProvider);

                ExposeRegisteredAppResources();

                var broker = serviceProvider.TryGetService<IMessageBroker>();
                broker?.Broadcast(this, new ApplicationBootCompleted());

                bootCompletedHandler?.Invoke(serviceProvider);

                var callbacks = serviceProvider.GetServices<IExpectBootCallback>();
                if (callbacks != null && callbacks.Any())
                {
                    foreach (var cb in callbacks)
                    {
                        cb.OnBootCompleted();
                    }
                }

                isBootCompleted = true;
            }
        }

        void ExposeRegisteredAppResources()
        {
            if (resourcesRegistrationHolder.Registrations.TryGetValue(Application.Current.GetType(), out HashSet<Type> services) && services.Any())
            {
                var conventions = (IConventionsHandler)serviceProvider.GetService(typeof(IConventionsHandler));
                foreach (var type in services)
                {
                    var instance = serviceProvider.GetService(type);
                    var key = conventions.GenerateServiceStaticResourceKey(type);
                    Application.Current.Resources.Add(key, instance);
                }
            }
        }

        readonly ResourcesRegistrationHolder resourcesRegistrationHolder = new ResourcesRegistrationHolder();

        /// <summary>
        /// Exposes the given service type as resource in the App resources.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        public ApplicationBootstrapper ExposeAsResource<TService>()
        {
            return ExposeAsResource(typeof(TService), Application.Current.GetType());
        }

        /// <summary>
        /// Exposes the given service type as resource in the supplied resource owner.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <returns></returns>
        public ApplicationBootstrapper ExposeAsResource<TService, TView>() where TView : FrameworkElement
        {
            return ExposeAsResource(typeof(TService), typeof(TView));
        }

        /// <summary>
        /// Exposes the given service type as resource in the supplied resource owner.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="resourceOwner">The resource owner.</param>
        /// <returns></returns>
        internal ApplicationBootstrapper ExposeAsResource(Type serviceType, Type resourceOwner)
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

            return this;
        }

        /// <summary>
        /// Using as current culture.
        /// </summary>
        /// <param name="currentCultureHandler">The current culture handler.</param>
        /// <returns></returns>
        public ApplicationBootstrapper UsingAsCurrentCulture(Func<CultureInfo> currentCultureHandler)
        {
            this.bootstrapConfiguration.UseCulture(_=> currentCultureHandler());

            return this;
        }

        /// <summary>
        /// Using as current UI culture.
        /// </summary>
        /// <param name="currentUICultureHandler">The current UI culture handler.</param>
        /// <returns></returns>
        public ApplicationBootstrapper UsingAsCurrentUICulture(Func<CultureInfo> currentUICultureHandler)
        {
            this.bootstrapConfiguration.UseUICulture(_=> currentUICultureHandler());

            return this;
        }

        void HandleSingletonApplicationStartup(SingletonApplicationStartupArgs args)
        {
            if (args.Scope != SingletonApplicationScope.NotSupported)
            {
                string mutexName = key;
                switch (args.Scope)
                {
                    case SingletonApplicationScope.Local:
                        mutexName = @"Local\" + mutexName;
                        break;

                    case SingletonApplicationScope.Global:
                        mutexName = @"Global\" + mutexName;
                        break;
                }

                mutex = new Mutex(false, mutexName);
                args.AllowStartup = mutex.WaitOne(TimeSpan.Zero, false);

                onSingletonApplicationStartup?.Invoke(args);
            }
        }

        Action<SingletonApplicationStartupArgs> onSingletonApplicationStartup;

        /// <summary>
        /// Called when a singleton application startup.
        /// </summary>
        /// <param name="onSingletonApplicationStartup">The singleton application startup handler.</param>
        /// <returns></returns>
        public ApplicationBootstrapper OnSingletonApplicationStartup(Action<SingletonApplicationStartupArgs> onSingletonApplicationStartup)
        {
            this.onSingletonApplicationStartup = onSingletonApplicationStartup;

            return this;
        }

        void OnBoot(IServiceProvider serviceProvider)
        {
            var broker = serviceProvider.GetService<IMessageBroker>();
            broker.Subscribe<ApplicationShutdownRequest>(this, InvocationModel.Safe, (s, m) =>
            {
                OnShutdownCore(ApplicationShutdownReason.UserRequest);
            });

            var args = new SingletonApplicationStartupArgs(singleton, key);
            HandleSingletonApplicationStartup(args);

            if (args.AllowStartup)
            {
                bootHandler?.Invoke(serviceProvider);
            }
            else
            {
                OnShutdownCore(ApplicationShutdownReason.MultipleInstanceNotAllowed);
            }
        }

        /// <summary>
        /// Boots this instance.
        /// </summary>
        public void Boot()
        {
            if (!isAutoBootEnabled && !isBootCompleted)
            {
                OnBoot();
            }
        }

        /// <summary>
        /// Shutdowns this application.
        /// </summary>
        public void Shutdown()
        {
            OnShutdownCore(ApplicationShutdownReason.UserRequest);
        }
        
        void OnBootCompleted(IServiceProvider serviceProvider)
        {
            var resolver = serviceProvider.GetService<IViewResolver>();

            Window showSplash()
            {
                var splashScreen = (Window)resolver.GetView(splashScreenConfiguration.SplashScreenViewType);
                Application.Current.MainWindow = splashScreen;

                splashScreen.WindowStartupLocation = splashScreenConfiguration.WindowStartupLocation;
                if (splashScreenConfiguration.MinWidth.HasValue)
                {
                    splashScreen.MinWidth = splashScreenConfiguration.MinWidth.Value;
                }

                if (splashScreenConfiguration.MinHeight.HasValue)
                {
                    splashScreen.MinHeight = splashScreenConfiguration.MinHeight.Value;
                }

                splashScreen.WindowStyle = splashScreenConfiguration.WindowStyle;
                splashScreen.SizeToContent = splashScreenConfiguration.SizeToContent;
                switch (splashScreen.SizeToContent)
                {
                    case SizeToContent.Manual:
                        splashScreen.Width = splashScreenConfiguration.Width;
                        splashScreen.Height = splashScreenConfiguration.Height;
                        break;

                    case SizeToContent.Height:
                        splashScreen.Width = splashScreenConfiguration.Width;
                        break;

                    case SizeToContent.Width:
                        splashScreen.Height = splashScreenConfiguration.Height;
                        break;
                }

                splashScreen.Show();

                return splashScreen;
            }

            void showShell()
            {
                if (shellViewType != null)
                {
                    var mainView = (Window)resolver.GetView(shellViewType);
                    Application.Current.MainWindow = mainView;

                    mainView.Show();
                }
            }

            if (isSplashScreenEnabled)
            {
                var splashScreen = showSplash();

                async Task action()
                {
                    var sw = Stopwatch.StartNew();
                    
                    await splashScreenConfiguration.StartupAsyncWork(serviceProvider)
                        .ConfigureAwait(true);
                    
                    sw.Stop();
                    var elapsed = (int)sw.ElapsedMilliseconds;
                    var remaining = splashScreenConfiguration.MinimumDelay - elapsed;
                    if (remaining > 0)
                    {
                        await Task.Delay(remaining)
                            .ConfigureAwait(true);
                    }
                }

                var startup = Task.Run(action);

                startup.ContinueWith(t =>
                {
                   if (t.IsFaulted)
                   {
                       OnUnhandledException(t.Exception);
                       throw t.Exception;
                   }

                   showShell();
                   splashScreen.Close();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                showShell();
            }
        }

        void OnShutdownCore(ApplicationShutdownReason reason)
        {
            var canceled = false;

            try
            {
                if (reason == ApplicationShutdownReason.UserRequest && isBootCompleted)
                {
                    var msg = new ApplicationShutdownRequested(reason);

                    var broker = serviceProvider.GetService<IMessageBroker>();
                    broker.Dispatch(this, msg);

                    canceled = msg.Cancel;

                    if (canceled)
                    {
                        broker.Broadcast(this, new ApplicationShutdownCanceled(reason));
                        return;
                    }
                }

                isShuttingDown = true;

                if (isBootCompleted)
                {
                    serviceProvider
                        .GetService<IMessageBroker>()
                        .Broadcast(this, new ApplicationShutdown(reason));

                    var callbacks = serviceProvider.GetServices<IExpectShutdownCallback>();
                    if (callbacks != null && callbacks.Any())
                    {
                        foreach (var cb in callbacks)
                        {
                            cb.OnShutdown(reason);
                        }
                    }
                }

                var args = new ApplicationShuttingDownArgs()
                {
                    Reason = reason,
                    IsBootCompleted = isBootCompleted
                };

                shutdownHandler?.Invoke(args);

                if (isBootCompleted)
                {
                    (serviceProvider as IDisposable)?.Dispose();
                }

                mutex?.Dispose();
                mutex = null;

            }
            finally
            {
                if (!canceled && reason != ApplicationShutdownReason.ApplicationRequest)
                {
                    Application.Current.Shutdown();
                }

                if (!canceled)
                {
                    serviceProvider = null;

                    RegionService.CurrentService = null;
                    RegionService.Conventions = null;
                }
            }
        }

        /// <summary>
        /// Registers this application as singleton.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public ApplicationBootstrapper RegisterAsSingleton(string key)
        {
            return RegisterAsSingleton(key, SingletonApplicationScope.Local);
        }

        /// <summary>
        /// Registers this application as singleton.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        public ApplicationBootstrapper RegisterAsSingleton(string key, SingletonApplicationScope scope)
        {
            this.key = key;
            singleton = scope;

            return this;
        }

        /// <summary>
        /// Overrides the shutdown mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public ApplicationBootstrapper OverrideShutdownMode(ShutdownMode mode)
        {
            this.shutdownMode = mode;

            return this;
        }

        /// <summary>
        /// Called when the application is booting.
        /// </summary>
        /// <param name="bootHandler">The boot handler.</param>
        /// <returns></returns>
        public ApplicationBootstrapper OnBoot(Action<IServiceProvider> bootHandler)
        {
            this.bootHandler = bootHandler;
            return this;
        }

        private Action<Exception> unhandledExceptionHandler;

        void OnUnhandledException(Exception exception)
        {
            unhandledExceptionHandler?.Invoke(exception);
        }

        /// <summary>
        /// Allows to inject an handler for not handled exception(s).
        /// </summary>
        /// <param name="unhandledExceptionHandler">The not handled exception handler.</param>
        /// <returns></returns>
        public ApplicationBootstrapper OnUnhandledException(Action<Exception> unhandledExceptionHandler)
        {
            this.unhandledExceptionHandler = unhandledExceptionHandler;
            return this;
        }

        /// <summary>
        /// Called when the boot process is completed.
        /// </summary>
        /// <param name="bootCompletedHandler">The boot completed handler.</param>
        /// <returns></returns>
        public ApplicationBootstrapper OnBootCompleted(Action<IServiceProvider> bootCompletedHandler)
        {
            this.bootCompletedHandler = bootCompletedHandler;
            return this;
        }

        /// <summary>
        /// Called when the application is shutting down.
        /// </summary>
        /// <param name="shutdownHandler">The shutdown handler.</param>
        /// <returns></returns>
        public ApplicationBootstrapper OnShutdown(Action<ApplicationShuttingDownArgs> shutdownHandler)
        {
            this.shutdownHandler = shutdownHandler;
            return this;
        }
    }
}
