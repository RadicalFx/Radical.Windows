using Radical.Windows.Controls;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Radical.Windows.Behaviors
{
    sealed class MultiThreadingBusyAdorner : OverlayAdorner
    {
        readonly BackgroundVisualHost _busyHost = null;
        readonly ContentPresenter userContentPresenter;

        MemoryStream ms = null;

        public MultiThreadingBusyAdorner(UIElement adornedElement, object userContent)
            : base(adornedElement)
        {
            userContentPresenter = BusyStatusManager.WrapUserContent(userContent);

            _busyHost = new BackgroundVisualHost(() =>
           {
               var s = (ContentPresenter)XamlReader.Load(ms);

               ms.Dispose();
               ms = null;

               return s;
           });

            AddLogicalChild(_busyHost);
            AddVisualChild(_busyHost);
        }

        protected override UIElement Content
        {
            get { return _busyHost; }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var brush = new SolidColorBrush(Color.FromArgb(100, 220, 220, 220));
            var rect = new Rect(new Point(0, 0), DesiredSize);

            drawingContext.DrawRectangle(brush, null, rect);

            base.OnRender(drawingContext);
        }

        internal void Setup()
        {
            ms = new MemoryStream();
            XamlWriter.Save(userContentPresenter, ms);
            ms.Flush();
            ms.Position = 0;

            _busyHost.Setup();
        }

        internal void Teardown()
        {
            _busyHost.Teardown();
        }
    }
}
