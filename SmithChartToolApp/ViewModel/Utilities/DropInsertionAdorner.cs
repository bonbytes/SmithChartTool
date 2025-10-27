using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;

namespace SmithChartToolApp.ViewModel.Utilities
{
    internal class DropInsertionAdorner : Adorner
    {
        private readonly Pen _pen;
        private double _x;
        private bool _visible;

        public int InsertionIndex { get; set; } = -1;

        public DropInsertionAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _pen = new Pen(Brushes.BlueViolet, 3);
            _pen.Freeze();
            IsHitTestVisible = false;
            _x = 0;
            _visible = false;
        }

        public void Show(double x)
        {
            _x = x;
            _visible = true;
            InvalidateVisual();
        }

        public void Hide()
        {
            if (_visible)
            {
                _visible = false;
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (!_visible)
                return;

            var adSize = AdornedElement.RenderSize;
            // vertical line spanning the adorned element height
            drawingContext.DrawLine(_pen, new Point(_x, 0), new Point(_x, adSize.Height));
        }
    }
}