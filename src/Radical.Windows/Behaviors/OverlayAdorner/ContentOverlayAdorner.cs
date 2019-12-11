namespace Radical.Windows.Behaviors
{
    using Radical.Windows.Controls;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    sealed class ContentOverlayAdorner : OverlayAdorner
    {
        private readonly ContentPresenter userContent;

        public ContentOverlayAdorner(UIElement adornedElement, object content) :
            base(adornedElement)
        {
            IsHitTestVisible = true;
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

            //WARN: if this is in a template the...
            AddVisualChild(userContent);
        }

        protected override UIElement Content
        {
            get { return userContent; }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Background != null)
            {
                var rect = new Rect(new Point(0, 0), DesiredSize);

                drawingContext.DrawRectangle(Background, null, rect);
            }

            base.OnRender(drawingContext);
        }

        public Brush Background { get; set; }
    }
}
