using System.Windows;
using System.Windows.Media;

namespace Radical.Windows.Controls
{
    /// <summary>
    /// A presentation source that connects a visual tree, created on a different thread, to a <see cref="HostVisual"/>.
    /// </summary>
    public class VisualTargetPresentationSource : PresentationSource
    {
        private readonly VisualTarget _visualTarget;
        private bool _isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualTargetPresentationSource"/> class.
        /// </summary>
        /// <param name="hostVisual">The host visual to connect the visual tree to.</param>
        public VisualTargetPresentationSource(HostVisual hostVisual)
        {
            _visualTarget = new VisualTarget(hostVisual);
            AddSource();
        }

        /// <summary>
        /// Gets the desired size of the root visual, computed when the root visual is set.
        /// </summary>
        public Size DesiredSize { get; private set; }

        /// <inheritdoc/>
        public override Visual RootVisual
        {
            get { return _visualTarget.RootVisual; }
            set
            {
                Visual oldRoot = _visualTarget.RootVisual;

                // Set the root visual of the VisualTarget.  This visual will
                // now be used to visually compose the scene.
                _visualTarget.RootVisual = value;

                // Tell the PresentationSource that the root visual has
                // changed.  This kicks off a bunch of stuff like the
                // Loaded event.
                RootChanged(oldRoot, value);

                // Kickoff layout...
                if (value is UIElement rootElement)
                {
                    rootElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    rootElement.Arrange(new Rect(rootElement.DesiredSize));

                    DesiredSize = rootElement.DesiredSize;
                }
                else
                {
                    DesiredSize = new Size(0, 0);
                }
            }
        }

        /// <inheritdoc/>
        protected override CompositionTarget GetCompositionTargetCore()
        {
            return _visualTarget;
        }

        /// <inheritdoc/>
        public override bool IsDisposed
        {
            get { return _isDisposed; }
        }

        internal void Dispose()
        {
            RemoveSource();
            _isDisposed = true;
        }
    }
}
