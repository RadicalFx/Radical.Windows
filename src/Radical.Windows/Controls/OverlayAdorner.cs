namespace Radical.Windows.Controls
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    public abstract class OverlayAdorner : Adorner
    {
        protected OverlayAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        protected abstract UIElement Content
        {
            get;
        }

        protected override Visual GetVisualChild(int index)
        {
            return Content;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Content.Measure(AdornedElement.RenderSize);

            return AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Content.Arrange(new Rect(finalSize));

            return finalSize;
        }
    }
}