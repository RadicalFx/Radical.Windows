using Radical.Windows.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    sealed class CueBannerAdorner : OverlayAdorner
    {
        private readonly ContentPresenter userContent;

        public CueBannerAdorner(UIElement adornedElement, object content) :
            base(adornedElement)
        {
            IsHitTestVisible = false;
            userContent = new ContentPresenter();

            var cueBannerText = content as string;
            if (cueBannerText != null)
            {
                userContent.Content = new TextBlock()
                {
                    FontStyle = FontStyles.Italic,
                    Text = cueBannerText,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(6, 0, 0, 0),
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Opacity = 0.7
                };
            }
            else
            {
                userContent.Content = content;
            }

            AddVisualChild(userContent);
        }

        protected override UIElement Content
        {
            get { return userContent; }
        }
    }
}
