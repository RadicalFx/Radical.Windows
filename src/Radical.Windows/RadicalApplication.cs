using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel.Messaging;
using Radical.Linq;
using Radical.Validation;
using Radical.Windows.Bootstrap;
using Radical.Windows.ComponentModel;
using Radical.Windows.Messaging;
using Radical.Windows.Regions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Radical.Windows
{
    /// <summary>
    /// The Radical application.
    /// </summary>
    public sealed class RadicalApplication
    {
        /// <summary>
        /// Create a Radical application with a life-cycle
        /// bound to the supplied WPF application, using
        /// the given configuration.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        internal static RadicalApplication BoundTo(Application application, BootstrapConfiguration bootstrapConfiguration)
        {
            Ensure.That(bootstrapConfiguration)
                .Named(nameof(bootstrapConfiguration))
                .IsNotNull();

            var services = new ServiceCollection();
            bootstrapConfiguration.ConfigureServicesHandler(services);
            bootstrapConfiguration.PopulateServiceCollection(services);
            var serviceProvider = services.BuildServiceProvider();
            bootstrapConfiguration.OnServiceProviderCreatedHandler(serviceProvider);

            var radicalApplication = new RadicalApplication(application, serviceProvider, bootstrapConfiguration);

            application.Startup += (s, e) =>
            {
                if (bootstrapConfiguration.IsAutoBootEnabled)
                {
                    radicalApplication.BootApplication();
                }
            };

            return radicalApplication;
        }

        internal static RadicalApplication ExternallyManagedBy(Application application, IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration)
        {
            Ensure.That(bootstrapConfiguration.IsAutoBootEnabled)
                .WithMessage("When using Generic Host auto boot cannot be disabled.")
                .Is(true);

            return new RadicalApplication(application, serviceProvider, bootstrapConfiguration);
        }

        readonly Application application;
        readonly BootstrapConfiguration bootstrapConfiguration;
        readonly IServiceProvider serviceProvider;
        bool isSessionEnding = false;
        bool isShuttingDown = false;
        bool isBootCompleted = false;
        Mutex singletonApplicationMutex;

        internal RadicalApplication(Application application, IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration)
        {
            this.application = application;
            this.serviceProvider = serviceProvider;
            this.bootstrapConfiguration = bootstrapConfiguration;

            application.SessionEnding += (s, e) =>
            {
                isSessionEnding = true;
            };

            application.DispatcherUnhandledException += (s, e) =>
            {
                var ex = e.Exception;
                bootstrapConfiguration.UnhandledExceptionHandler(ex);
            };

            application.Exit += (s, e) =>
            {
                if (!isShuttingDown)
                {
                    var reason = isSessionEnding ? ApplicationShutdownReason.SessionEnding : ApplicationShutdownReason.ApplicationRequest;
                    OnShutdownCore(reason);
                }
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                bootstrapConfiguration.UnhandledExceptionHandler(ex);
            };
        }

        internal void BootApplication()
        {
            var features = serviceProvider.GetServices<IFeature>();
            foreach (var feature in features)
            {
                feature.Setup(serviceProvider, bootstrapConfiguration);
            }

            if (bootstrapConfiguration.ShutdownMode != null)
            {
                application.ShutdownMode = bootstrapConfiguration.ShutdownMode.Value;
            }

            var broker = serviceProvider.GetService<IMessageBroker>();
            broker.Subscribe<ApplicationShutdownRequest>(this, InvocationModel.Safe, (s, m) =>
            {
                OnShutdownCore(ApplicationShutdownReason.UserRequest);
            });

            var args = new SingletonApplicationStartupArgs(bootstrapConfiguration.SingletonApplicationScope, bootstrapConfiguration.SingletonRegistrationKey);
            HandleSingletonApplicationStartup(args);

            if (args.AllowStartup)
            {
                bootstrapConfiguration.BootingHandler(serviceProvider);
            }
            else
            {
                OnShutdownCore(ApplicationShutdownReason.MultipleInstanceNotAllowed);
            }

            if (!isShuttingDown)
            {
                OnBootCompleted(serviceProvider);

                bootstrapConfiguration.ExposeRegisteredAppResources(serviceProvider, application);

                broker.Broadcast(this, new ApplicationBootCompleted());

                bootstrapConfiguration.BootCompletedHandler(serviceProvider);

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

        void HandleSingletonApplicationStartup(SingletonApplicationStartupArgs args)
        {
            if (args.Scope != SingletonApplicationScope.NotSupported)
            {
                string mutexName = @"Local\" + args.SingletonRegistrationKey;
                if (args.Scope == SingletonApplicationScope.Global)
                {
                    mutexName = @"Global\" + args.SingletonRegistrationKey;
                }

                singletonApplicationMutex = new Mutex(false, mutexName);
                args.AllowStartup = singletonApplicationMutex.WaitOne(TimeSpan.Zero, false);

                bootstrapConfiguration.OnSingletonApplicationStartupHandler(args);
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

                bootstrapConfiguration.ShuttingDownHandler(new ApplicationShuttingDownArgs()
                {
                    Reason = reason,
                    IsBootCompleted = isBootCompleted
                });
            }
            finally
            {
                if (!canceled)
                {
                    if (reason != ApplicationShutdownReason.ApplicationRequest)
                    {
                        application.Shutdown();
                    }

                    singletonApplicationMutex?.Dispose();
                    singletonApplicationMutex = null;

                    RegionService.CurrentService = null;
                    RegionService.Conventions = null;
                }
            }
        }

        /// <summary>
        /// Boots this instance.
        /// </summary>
        public void Boot()
        {
            if (!bootstrapConfiguration.IsAutoBootEnabled && !isBootCompleted)
            {
                BootApplication();
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
                var splashScreenConfiguration = bootstrapConfiguration.SplashScreenConfiguration;
                var splashScreen = (Window)resolver.GetView(splashScreenConfiguration.SplashScreenViewType);
                application.MainWindow = splashScreen;

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
                if (bootstrapConfiguration.ShellViewType != null)
                {
                    var mainView = (Window)resolver.GetView(bootstrapConfiguration.ShellViewType);
                    application.MainWindow = mainView;

                    mainView.Show();
                }
            }

            if (bootstrapConfiguration.IsSplashScreenEnabled)
            {
                var splashScreen = showSplash();

                async Task action()
                {
                    var sw = Stopwatch.StartNew();

                    await bootstrapConfiguration.SplashScreenConfiguration.StartupAsyncWork(serviceProvider)
                        .ConfigureAwait(true);

                    sw.Stop();
                    var elapsed = (int)sw.ElapsedMilliseconds;
                    var remaining = bootstrapConfiguration.SplashScreenConfiguration.MinimumDelay - elapsed;
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
                        bootstrapConfiguration.UnhandledExceptionHandler(t.Exception);
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
    }
}