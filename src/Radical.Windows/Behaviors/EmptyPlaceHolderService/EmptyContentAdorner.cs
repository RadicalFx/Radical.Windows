using Radical.Windows.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Radical.Windows.Behaviors
{
    class EmptyContentAdorner : OverlayAdorner
    {
        private readonly ContentPresenter userContent;

        public EmptyContentAdorner(UIElement adornedElement, object content) :
            base(adornedElement)
        {
            IsHitTestVisible = false;
            userContent = new ContentPresenter();

            if (content is string emptyText)
            {
                userContent.Content = new TextBlock()
                {
                    FontStyle = FontStyles.Italic,
                    Text = emptyText,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 25, 0, 0),
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
