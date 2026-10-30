namespace Radical.Windows.Controls
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// Base class for adorners that overlay the adorned element with their content.
    /// </summary>
    public abstract class OverlayAdorner : Adorner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The element to adorn.</param>
        protected OverlayAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        /// <summary>
        /// Gets the content displayed by the adorner.
        /// </summary>
        protected abstract UIElement Content
        {
            get;
        }

        /// <inheritdoc/>
        protected override Visual GetVisualChild(int index)
        {
            return Content;
        }

        /// <inheritdoc/>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size constraint)
        {
            Content.Measure(AdornedElement.RenderSize);

            return AdornedElement.RenderSize;
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Content.Arrange(new Rect(finalSize));

            return finalSize;
        }
    }
}