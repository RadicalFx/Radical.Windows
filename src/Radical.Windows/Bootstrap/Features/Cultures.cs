using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace Radical.Windows.Bootstrap.Features
{
    class Cultures : IFeature
    {
        public void Setup(IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration)
        {
            var currentCulture = bootstrapConfiguration.CultureConfig(serviceProvider);
            var currentUICulture = bootstrapConfiguration.UICultureConfig(serviceProvider);

            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentUICulture;

            if (UnitTestDetector.IsTest) 
            {
                return;
            }

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
    }

    /// <summary>
    /// Detect if we are running as part of a unit test.
    /// This is DIRTY and should only be used if absolutely necessary 
    /// as its usually a sign of bad design.
    /// </summary>    
    static class UnitTestDetector
    {
        static UnitTestDetector()
        {
            IsTest = AppDomain.CurrentDomain.GetAssemblies().Any(assembly =>
            {
                return assembly.FullName.StartsWith("Microsoft.VisualStudio.TestPlatform", StringComparison.InvariantCultureIgnoreCase);
            });
        }

        public static bool IsTest { get; }
    }
}
