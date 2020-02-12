using Radical.ComponentModel;
using Radical.ComponentModel.Messaging;
using Radical.Linq;
using Radical.Reflection;
using Radical.Windows.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Radical.Windows.Bootstrap
{
    /// <summary>
    /// Application bootstrap conventions
    /// </summary>
    public class BootstrapConventions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapConventions" /> class.
        /// </summary>
        public BootstrapConventions()
        {
            DefaultIsConcreteType = t => !t.IsInterface && !t.IsAbstract && !t.IsGenericType;
            IsConcreteType = t => DefaultIsConcreteType(t);

            DefaultIsService = t => IsConcreteType(t) && t.Namespace.IsLike("*.Services");
            IsService = t => DefaultIsService(t);

            DefaultAllowServiceOverride = t => t.Namespace.IsLike("Radical.*.Services");
            AllowServiceOverride = t => DefaultAllowServiceOverride(t);

            DefaultSelectServiceContracts = type =>
            {
                var types = new HashSet<Type>(type.GetInterfaces());
                if (types.None() || type.IsAttributeDefined<ContractAttribute>())
                {
                    types.Add(type);
                }

                return types;
            };
            SelectServiceContracts = type => DefaultSelectServiceContracts(type);

            DefaultIsMessageHandler = t =>
            {
                return t.Namespace != null && t.Namespace.IsLike(new string[]
                    {
                        "*.Messaging.Handlers",
                        "*.Messaging.Handlers.*"
                    })
                    && t.Is<IHandleMessage>();
            };
            IsMessageHandler = t => DefaultIsMessageHandler(t);

            DefaultSelectMessageHandlerContracts = type => type.GetInterfaces().Except(new[] { typeof(INeedSafeSubscription) });
            SelectMessageHandlerContracts = type => DefaultSelectMessageHandlerContracts(type);

            DefaultIsViewModel = t => IsConcreteType(t) && t.FullName.IsLike("*.Presentation.*ViewModel");
            IsViewModel = t => DefaultIsViewModel(t);

            DefaultIsShellViewModel = (services, implementation) =>
            {
                return services.Any(t => t.Name.IsLike("Main*") || t.Name.IsLike("Shell*"));
            };
            IsShellViewModel = (services, implementation) => DefaultIsShellViewModel(services, implementation);

            DefaultSelectViewModelContracts = type => new[] { type };
            SelectViewModelContracts = type => DefaultSelectViewModelContracts(type);

            DefaultIsView = t => IsConcreteType(t) && t.FullName.IsLike("*.Presentation.*View");
            IsView = t => DefaultIsView(t);

            DefaultIsShellView = (services, implementation) =>
            {
                return services.Any(t => t.Name.IsLike("Main*") || t.Name.IsLike("Shell*"));
            };
            IsShellView = (services, implementation) => DefaultIsShellView(services, implementation);

            DefaultSelectViewContracts = type => new[] { type };
            SelectViewContracts = type => DefaultSelectViewContracts(type);

            DefaultGetInterestedRegionNameIfAny = type =>
            {
                if (IsView(type))
                {

                    if (type.IsAttributeDefined<InjectViewInRegionAttribute>())
                    {
                        return type.GetAttribute<InjectViewInRegionAttribute>().Named;
                    }

                    if (type.Namespace.IsLike("*.Presentation.Partial.*"))
                    {
                        var regionName = type.Namespace.Split('.').Last();
                        return regionName;
                    }
                }

                return null;
            };
            GetInterestedRegionNameIfAny = type => DefaultGetInterestedRegionNameIfAny(type);

            DefaultIsExcluded = t =>
            {
                return t.IsAttributeDefined<DisableAutomaticRegistrationAttribute>();
            };
            IsExcluded = t => DefaultIsExcluded(t);

            DefaultAssemblyFileScanPatterns = entryAssembly =>
            {
                return new[] 
                {
                    $"{entryAssembly.GetName().Name}*.dll", 
                    "Radical.*.dll" 
                };
            };
            AssemblyFileScanPatterns = entryAssembly => DefaultAssemblyFileScanPatterns(entryAssembly);

            DefaultIncludeAssemblyInContainerScan = assembly => true;
            IncludeAssemblyInContainerScan = assembly => DefaultIncludeAssemblyInContainerScan(assembly);

            DefaultIgnorePropertyInjection = pi =>
            {
                var isDefined = pi.IsAttributeDefined<IgnorePropertyInjectionAttribue>();
                return isDefined;
            };
            IgnorePropertyInjection = pi => DefaultIgnorePropertyInjection(pi);

            DefaultIgnoreViewPropertyInjection = pi =>
            {
                return true;
            };
            IgnoreViewPropertyInjection = pi => DefaultIgnoreViewPropertyInjection(pi);

            DefaultIgnoreViewModelPropertyInjection = pi =>
            {
                return true;
            };
            IgnoreViewModelPropertyInjection = pi => DefaultIgnoreViewModelPropertyInjection(pi);
        }

        /// <summary>
        /// Default: Gets or sets the type of the is concrete.
        /// </summary>
        /// <value>
        /// The type of the is concrete.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> DefaultIsConcreteType { get; private set; }

        /// <summary>
        /// Gets or sets the type of the is concrete.
        /// </summary>
        /// <value>
        /// The type of the is concrete.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> IsConcreteType { get; set; }

        /// <summary>
        /// Default: Gets or sets the is service.
        /// </summary>
        /// <value>
        /// The is service.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> DefaultIsService { get; private set; }

        /// <summary>
        /// Gets or sets the is service.
        /// </summary>
        /// <value>
        /// The is service.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> IsService { get; set; }

        /// <summary>
        /// Default: Gets or sets if a service can be overridden.
        /// </summary>
        /// <value>
        /// The is service.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> DefaultAllowServiceOverride { get; private set; }

        /// <summary>
        /// Gets or sets if a service can be overridden.
        /// </summary>
        /// <value>
        /// The is service.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> AllowServiceOverride { get; set; }

        /// <summary>
        /// Default: Gets or sets the select service contracts.
        /// </summary>
        /// <value>
        /// The select service contracts.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, IEnumerable<Type>> DefaultSelectServiceContracts { get; private set; }

        /// <summary>
        /// Gets or sets the select service contracts.
        /// </summary>
        /// <value>
        /// The select service contracts.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, IEnumerable<Type>> SelectServiceContracts { get; set; }

        /// <summary>
        /// Default: Gets or sets the is message handler.
        /// </summary>
        /// <value>
        /// The is message handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> DefaultIsMessageHandler { get; private set; }

        /// <summary>
        /// Gets or sets the is message handler.
        /// </summary>
        /// <value>
        /// The is message handler.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> IsMessageHandler { get; set; }

        /// <summary>
        /// Default: Gets or sets the select message handler contracts.
        /// </summary>
        /// <value>
        /// The select message handler contracts.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, IEnumerable<Type>> DefaultSelectMessageHandlerContracts { get; private set; }

        /// <summary>
        /// Gets or sets the select message handler contracts.
        /// </summary>
        /// <value>
        /// The select message handler contracts.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, IEnumerable<Type>> SelectMessageHandlerContracts { get; set; }

        /// <summary>
        /// Default: Gets or sets the is view.
        /// </summary>
        /// <value>
        /// The is view.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> DefaultIsView { get; private set; }

        /// <summary>
        /// Gets or sets the is view.
        /// </summary>
        /// <value>
        /// The is view.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> IsView { get; set; }

        /// <summary>
        /// Default: Gets or sets the is view model.
        /// </summary>
        /// <value>
        /// The is view model.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> DefaultIsViewModel { get; private set; }

        /// <summary>
        /// Gets or sets the is view model.
        /// </summary>
        /// <value>
        /// The is view model.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Type> IsViewModel { get; set; }

        /// <summary>
        /// Default: Gets or sets the is shell view.
        /// </summary>
        /// <value>
        /// The is shell view.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<IEnumerable<Type>, Type, bool> DefaultIsShellView { get; private set; }

        /// <summary>
        /// Gets or sets the is shell view.
        /// </summary>
        /// <value>
        /// The is shell view.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<IEnumerable<Type>, Type, bool> IsShellView { get; set; }

        /// <summary>
        /// Default: Gets or sets the is shell view model.
        /// </summary>
        /// <value>
        /// The is shell view model.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<IEnumerable<Type>, Type, bool> DefaultIsShellViewModel { get; private set; }

        /// <summary>
        /// Gets or sets the is shell view model.
        /// </summary>
        /// <value>
        /// The is shell view model.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<IEnumerable<Type>, Type, bool> IsShellViewModel { get; set; }

        /// <summary>
        /// Default: Gets or sets the select view contracts.
        /// </summary>
        /// <value>
        /// The select view contracts.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, IEnumerable<Type>> DefaultSelectViewContracts { get; private set; }

        /// <summary>
        /// Gets or sets the select view contracts.
        /// </summary>
        /// <value>
        /// The select view contracts.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, IEnumerable<Type>> SelectViewContracts { get; set; }

        /// <summary>
        /// Default: Gets or sets the select view model contracts.
        /// </summary>
        /// <value>
        /// The select view model contracts.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, IEnumerable<Type>> DefaultSelectViewModelContracts { get; private set; }

        /// <summary>
        /// Gets or sets the select view model contracts.
        /// </summary>
        /// <value>
        /// The select view model contracts.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, IEnumerable<Type>> SelectViewModelContracts { get; set; }

        /// <summary>
        /// Default: Gets or sets the get interested region name if any.
        /// </summary>
        /// <value>
        /// The get interested region name if any.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, string> DefaultGetInterestedRegionNameIfAny { get; private set; }

        /// <summary>
        /// Gets or sets the get interested region name if any.
        /// </summary>
        /// <value>
        /// The get interested region name if any.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, string> GetInterestedRegionNameIfAny { get; set; }

        /// <summary>
        /// Default: Gets or sets the is excluded.
        /// </summary>
        /// <value>
        /// The is excluded.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, bool> DefaultIsExcluded { get; private set; }

        /// <summary>
        /// Gets or sets the is excluded.
        /// </summary>
        /// <value>
        /// The is excluded.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Type, bool> IsExcluded { get; set; }


        /// <summary>
        /// Default: Gets or sets the assembly file scan patterns.
        /// </summary>
        /// <value>
        /// The assembly file scan patterns.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Assembly, IEnumerable<string>> DefaultAssemblyFileScanPatterns { get; private set; }

        /// <summary>
        /// Gets or sets the assembly file scan patterns.
        /// </summary>
        /// <value>
        /// The assembly file scan patterns.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<Assembly, IEnumerable<string>> AssemblyFileScanPatterns { get; set; }

        /// <summary>
        /// Default: Gets or sets the include assembly in container scan.
        /// </summary>
        /// <value>
        /// The include assembly in container scan.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Assembly> DefaultIncludeAssemblyInContainerScan { get; private set; }

        /// <summary>
        /// Gets or sets the include assembly in container scan.
        /// </summary>
        /// <value>
        /// The include assembly in container scan.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Predicate<Assembly> IncludeAssemblyInContainerScan { get; set; }

        /// <summary>
        /// Default: Gets or sets the predicate that determines if a property is injectable or not.
        /// </summary>
        /// <value>
        /// The injectable properties predicate.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<PropertyInfo, bool> DefaultIgnorePropertyInjection { get; private set; }

        /// <summary>
        /// Gets or sets the predicate that determines if a property is injectable or not.
        /// </summary>
        /// <value>
        /// The injectable properties predicate.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<PropertyInfo, bool> IgnorePropertyInjection { get; set; }

        /// <summary>
        /// Default: Gets or sets the predicate that determines if a property of a View is injectable or not.
        /// </summary>
        /// <value>
        /// The injectable properties predicate.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<PropertyInfo, bool> DefaultIgnoreViewPropertyInjection { get; private set; }

        /// <summary>
        /// Gets or sets the predicate that determines if a property of a View is injectable or not.
        /// </summary>
        /// <value>
        /// The injectable properties predicate.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<PropertyInfo, bool> IgnoreViewPropertyInjection { get; set; }

        /// <summary>
        /// Default: Gets or sets the predicate that determines if a property of a ViewModel is injectable or not.
        /// </summary>
        /// <value>
        /// The injectable properties predicate.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<PropertyInfo, bool> DefaultIgnoreViewModelPropertyInjection { get; private set; }

        /// <summary>
        /// Gets or sets the predicate that determines if a property of a ViewModel is injectable or not.
        /// </summary>
        /// <value>
        /// The injectable properties predicate.
        /// </value>
        [IgnorePropertyInjectionAttribue]
        public Func<PropertyInfo, bool> IgnoreViewModelPropertyInjection { get; set; }
    }
}
