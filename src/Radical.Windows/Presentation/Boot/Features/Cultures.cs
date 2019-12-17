using System;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace Radical.Windows.Presentation.Boot.Features
{
    class Cultures : IFeature
    {
        public void Setup(IServiceProvider serviceProvider, ApplicationSettings applicationSettings)
        {
            var currentCulture = applicationSettings.CurrentCultureHandler();
            var currentUICulture = applicationSettings.CurrentUICultureHandler();

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
