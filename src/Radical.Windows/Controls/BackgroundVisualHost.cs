using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Radical.Windows.Controls
{
    public class BackgroundVisualHost : FrameworkElement
    {
        private ThreadedVisualHelper _threadedHelper = null;
        private HostVisual _hostVisual = null;

        readonly Func<Visual> createContent;

        public BackgroundVisualHost(Func<Visual> createContent)
        {
            this.createContent = createContent;
        }

        internal void Setup()
        {
            CreateContentHelper();
        }

        internal void Teardown()
        {
            HideContentHelper();
        }

        protected override int VisualChildrenCount
        {
            get { return _hostVisual != null ? 1 : 0; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (_hostVisual != null && index == 0)
                return _hostVisual;

            throw new IndexOutOfRangeException("index");
        }

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                if (_hostVisual != null)
                    yield return _hostVisual;
            }
        }

        private void CreateContentHelper()
        {
            _threadedHelper = new ThreadedVisualHelper(createContent, SafeInvalidateMeasure);
            _hostVisual = _threadedHelper.HostVisual;
        }

        private void SafeInvalidateMeasure()
        {
            Dispatcher.BeginInvoke(new Action(InvalidateMeasure), DispatcherPriority.Loaded);
        }

        private void HideContentHelper()
        {
            if (_threadedHelper != null)
            {
                _threadedHelper.Exit();
                _threadedHelper = null;
                InvalidateMeasure();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_threadedHelper != null)
            {
                return _threadedHelper.DesiredSize;
            }

            return base.MeasureOverride(availableSize);
        }

        private class ThreadedVisualHelper
        {
            readonly HostVisual _hostVisual = null;
            readonly AutoResetEvent _sync = new AutoResetEvent(false);
            readonly Func<Visual> _createContent;
            readonly Action _invalidateMeasure;

            public HostVisual HostVisual { get { return _hostVisual; } }
            public Size DesiredSize { get; private set; }
            private Dispatcher Dispatcher { get; set; }

            public ThreadedVisualHelper(Func<Visual> createContent, Action invalidateMeasure)
            {
                _hostVisual = new HostVisual();
                _createContent = createContent;
                _invalidateMeasure = invalidateMeasure;

                Thread backgroundUi = new Thread(CreateAndShowContent);
                backgroundUi.SetApartmentState(ApartmentState.STA);
                backgroundUi.Name = "BackgroundVisualHostThread";
                backgroundUi.IsBackground = true;
                backgroundUi.Start();

                _sync.WaitOne();
            }

            public void Exit()
            {
                Dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
            }

            private void CreateAndShowContent()
            {
                Dispatcher = Dispatcher.CurrentDispatcher;
                var source = new VisualTargetPresentationSource(_hostVisual);
                _sync.Set();
                source.RootVisual = _createContent();
                DesiredSize = source.DesiredSize;
                _invalidateMeasure();

                Dispatcher.Run();
                source.Dispose();
            }
        }
    }
}
