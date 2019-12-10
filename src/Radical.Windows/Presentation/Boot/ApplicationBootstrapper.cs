using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel.Messaging;
using Radical.Diagnostics;
using Radical.Helpers;
using Radical.Validation;
using Radical.Reflection;
using Radical.Windows.Presentation.ComponentModel;
using Radical.Windows.Presentation.Messaging;
using Radical.Windows.Presentation.Regions;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Radical.Linq;

namespace Radical.Windows.Presentation.Boot
{
    /// <summary>
    /// The application bootstrapper. Provides a way to dramatically simplify the
    /// application boot process.
    /// </summary>
    public class ApplicationBootstrapper
    {
        static readonly TraceSource logger = new TraceSource(typeof(ApplicationBootstrapper).Name);

        Type shellViewType = null;
        IServiceProvider serviceProvider;

        bool isAutoBootEnabled = true;
        bool isBootCompleted;

        private Action<IServiceProvider> bootCompletedHandler;
        private Action<ApplicationShutdownArgs> shutdownHandler;
        private Action<IServiceProvider> bootHandler;
        ShutdownMode? mode = null;
        Mutex mutex;
        string key;
        SingletonApplicationScope singleton = SingletonApplicationScope.NotSupported;
        AssemblyScanner assemblyScanner = new AssemblyScanner();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBootstrapper"/> class.
        /// </summary>
        public ApplicationBootstrapper()
            : this(new ServiceCollection())
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBootstrapper"/> class.
        /// </summary>
        public ApplicationBootstrapper(IServiceCollection services)
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
                    OnBoot(services);
                }
            };

            Application.Current.SessionEnding += (s, e) =>
            {
                IsSessionEnding = true;
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
                if (!IsShuttingDown)
                {
                    var reason = IsSessionEnding ? ApplicationShutdownReason.SessionEnding : ApplicationShutdownReason.ApplicationRequest;
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

        /// <summary>
        /// Setups the UI composition engine.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected virtual void SetupUICompositionEngine(IServiceProvider serviceProvider)
        {
            RegionService.CurrentService = serviceProvider.GetService<IRegionService>();
            RegionService.Conventions = serviceProvider.GetService<IConventionsHandler>();
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

        void OnBoot(IServiceCollection services)
        {
            var conventions = new BootstrapConventions();
            onBeforeInstall?.Invoke(conventions, assemblyScanner);
            services.AddSingleton(conventions);

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

            var collector = serviceProvider.GetRequiredService<Installers.Collector>();
            var broker = serviceProvider.GetRequiredService<IMessageBroker>();

            foreach (var entry in collector.Entries)
            {
                var invocationModel = entry.Implementation.Is<INeedSafeSubscription>() ?
                    InvocationModel.Safe :
                    InvocationModel.Default;

                entry.Implementation.GetInterfaces()
                    .Where(i => i.Is<IHandleMessage>() && i.IsGenericType)
                    .ForEach(genericHandler =>
                    {
                        var messageType = genericHandler.GetGenericArguments().Single();
                        broker.Subscribe(this, messageType, invocationModel, (s, msg) =>
                        {
                            var handler = serviceProvider.GetService(entry.Contracts.First()) as IHandleMessage;

                            if (handler.ShouldHandle(s, msg))
                            {
                                handler.Handle(s, msg);
                            }
                        });
                    });
            }

            SetupUICompositionEngine(serviceProvider);

            if (mode != null && mode.HasValue)
            {
                Application.Current.ShutdownMode = mode.Value;
            }

            InitializeCurrentPrincipal();
            InitializeCultures();

            OnBoot(serviceProvider);

            if (!IsShuttingDown)
            {
                OnBootCompleted(serviceProvider);

                //var broker = serviceProvider.TryGetService<IMessageBroker>();
                //broker?.Broadcast(this, new ApplicationBootCompleted());

                broker.Broadcast(this, new ApplicationBootCompleted());

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

        Func<CultureInfo> currentCultureHandler = () => CultureInfo.CurrentCulture;

        /// <summary>
        /// Using as current culture.
        /// </summary>
        /// <param name="currentCultureHandler">The current culture handler.</param>
        /// <returns></returns>
        public ApplicationBootstrapper UsingAsCurrentCulture(Func<CultureInfo> currentCultureHandler)
        {
            this.currentCultureHandler = currentCultureHandler;

            return this;
        }

        Func<CultureInfo> currentUICultureHandler = () => CultureInfo.CurrentUICulture;

        /// <summary>
        /// Using as current UI culture.
        /// </summary>
        /// <param name="currentUICultureHandler">The current UI culture handler.</param>
        /// <returns></returns>
        public ApplicationBootstrapper UsingAsCurrentUICulture(Func<CultureInfo> currentUICultureHandler)
        {
            this.currentUICultureHandler = currentUICultureHandler;

            return this;
        }

        /// <summary>
        /// Initializes the current principal.
        /// </summary>
        protected virtual void InitializeCurrentPrincipal()
        {
            Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        }

        /// <summary>
        /// Initializes the cultures.
        /// </summary>
        protected virtual void InitializeCultures()
        {
            var currentCulture = currentCultureHandler();
            var currentUICulture = currentUICultureHandler();

            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentUICulture;

            var xmlLang = XmlLanguage.GetLanguage(currentCulture.IetfLanguageTag);
            FrameworkElement.LanguageProperty.OverrideMetadata
            (
                forType: typeof(FrameworkElement),
                typeMetadata: new FrameworkPropertyMetadata(xmlLang)
            );

            var fd = currentUICulture.TextInfo.IsRightToLeft ?
                FlowDirection.RightToLeft :
                FlowDirection.LeftToRight;

            FrameworkElement.FlowDirectionProperty.OverrideMetadata
            (
                forType: typeof(FrameworkElement),
                typeMetadata: new FrameworkPropertyMetadata(fd)
            );
        }

        /// <summary>
        /// Handles the singleton application scope.
        /// </summary>
        /// <param name="args">The args.</param>
        protected virtual void HandleSingletonApplicationStartup(SingletonApplicationStartupArgs args)
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

        /// <summary>
        /// Called in order to execute the boot process.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected virtual void OnBoot(IServiceProvider serviceProvider)
        {
            var broker = serviceProvider.GetService<IMessageBroker>();
            broker.Subscribe<ApplicationShutdownRequest>(this, InvocationModel.Safe, (s, m) =>
            {
                OnShutdownCore(ApplicationShutdownReason.UserRequest);
            });

            var args = new SingletonApplicationStartupArgs(singleton);
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
                OnBoot(new ServiceCollection());
            }
        }

        // <summary>
        /// Boots this instance.
        /// </summary>
        public void Boot(IServiceCollection services)
        {
            if (!isAutoBootEnabled && !isBootCompleted)
            {
                OnBoot(services);
            }
        }

        /// <summary>
        /// Shutdowns this application.
        /// </summary>
        public void Shutdown()
        {
            OnShutdownCore(ApplicationShutdownReason.UserRequest);
        }

        /// <summary>
        /// Called when the boot process has been completed.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected virtual void OnBootCompleted(IServiceProvider serviceProvider)
        {
            var resolver = serviceProvider.GetService<IViewResolver>();

            Func<Window> showSplash = () =>
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
            };

            Action showShell = () =>
            {
                if (shellViewType != null)
                {
                    var mainView = (Window)resolver.GetView(shellViewType);
                    Application.Current.MainWindow = mainView;

                    mainView.Show();
                }
            };

            if (isSplashScreenEnabled)
            {
                var splashScreen = showSplash();

                Action action = () =>
                {
                    var sw = Stopwatch.StartNew();
                    splashScreenConfiguration.StartupAsyncWork(serviceProvider);
                    sw.Stop();
                    var elapsed = (int)sw.ElapsedMilliseconds;
                    var remaining = splashScreenConfiguration.MinimumDelay - elapsed;
                    if (remaining > 0)
                    {
#if FX40
                        Thread.Sleep( remaining );
#else
                        Task.Delay(remaining);
#endif
                    }
                };

#if FX40
                var startup = Task.Factory.StartNew( action );
#else
                var startup = Task.Run(action);
#endif

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
                    //messaggio per notificare ed eventualmente cancellare
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

                IsShuttingDown = true;

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

                var args = new ApplicationShutdownArgs()
                {
                    Reason = reason,
                    IsBootCompleted = isBootCompleted
                };

                OnShutdown(args);
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
        /// Called when the application shutdowns.
        /// </summary>
        protected virtual void OnShutdown(ApplicationShutdownArgs e)
        {

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
            this.mode = mode;

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

        /// <summary>
        /// Gets a value indicating whether the operating system session is ending.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the operating system session is ending; otherwise, <c>false</c>.
        /// </value>
        protected bool IsSessionEnding
        {
            get;
            private set;
        }

        private Action<Exception> unhandledExceptionHandler;

        /// <summary>
        /// Called when a not handled exception occurs.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected virtual void OnUnhandledException(Exception exception)
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
        /// Gets a value indicating whether this application is shutting down.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this application is shutting down; otherwise, <c>false</c>.
        /// </value>
        protected bool IsShuttingDown
        {
            get;
            private set;
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
        public ApplicationBootstrapper OnShutdown(Action<ApplicationShutdownArgs> shutdownHandler)
        {
            this.shutdownHandler = shutdownHandler;
            return this;
        }
    }
}
