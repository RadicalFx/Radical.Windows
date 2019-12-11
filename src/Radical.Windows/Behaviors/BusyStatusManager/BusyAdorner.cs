using Radical.Windows.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Radical.Windows.Behaviors
{
    sealed class BusyAdorner : OverlayAdorner
    {
        private readonly ContentPresenter userContentPresenter;

        public BusyAdorner(UIElement adornedElement, object userContent)
            : base(adornedElement)
        {
            userContentPresenter = BusyStatusManager.WrapUserContent(userContent);
            AddVisualChild(userContentPresenter);
        }

        protected override UIElement Content
        {
            get { return userContentPresenter; }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var brush = new SolidColorBrush(Color.FromArgb(100, 220, 220, 220));
            var rect = new Rect(new Point(0, 0), DesiredSize);

            drawingContext.DrawRectangle(brush, null, rect);

            base.OnRender(drawingContext);
        }
    }
}
