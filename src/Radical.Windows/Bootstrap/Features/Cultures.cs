using System;
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
}
