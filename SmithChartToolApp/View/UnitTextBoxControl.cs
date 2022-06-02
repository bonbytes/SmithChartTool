using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmithChartToolApp.View
{
    public class UnitTextBoxControl : TextBox
    {

        private FormattedText _unit;
        private Rect _unitBounds;

        public static DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(UnitTextBoxControl), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static DependencyProperty UnitPaddingProperty = DependencyProperty.Register( "UnitPadding", typeof(Thickness), typeof(UnitTextBoxControl), new FrameworkPropertyMetadata(new Thickness(5d, 0d, 0d, 0d), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        public Thickness UnitPadding
        {
            get { return (Thickness)GetValue(UnitPaddingProperty); }
            set { SetValue(UnitPaddingProperty, value); }
        }

        public static DependencyProperty TextBoxWidthProperty = DependencyProperty.Register("TextBoxWidth", typeof(double), typeof(UnitTextBoxControl), new FrameworkPropertyMetadata( double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double TextBoxWidth
        {
            get { return (double)GetValue(TextBoxWidthProperty); }
            set { SetValue(TextBoxWidthProperty, value); }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ForegroundProperty)
                EnsureUnitText(true);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var textBoxWidth = this.TextBoxWidth;
            var unit = EnsureUnitText(invalidate: true);
            var padding = this.UnitPadding;

            if (unit != null)
            {
                var unitWidth = unit.Width + padding.Left + padding.Right;
                var unitHeight = unit.Height + padding.Top + padding.Bottom;
                constraint = new Size( constraint.Width - unitWidth, Math.Max(constraint.Height, unitHeight));
            }

            var hasFixedTextBoxWidth = !double.IsNaN(textBoxWidth) && !double.IsInfinity(textBoxWidth);

            if (hasFixedTextBoxWidth)
                constraint = new Size(textBoxWidth, constraint.Height);

            var baseSize = base.MeasureOverride(constraint);
            var baseWidth = hasFixedTextBoxWidth ? textBoxWidth : baseSize.Width;

            if (unit != null)
            {
                var unitWidth = unit.Width + padding.Left + padding.Right;
                var unitHeight = unit.Height + padding.Top + padding.Bottom;

                return new Size( baseWidth + unitWidth, Math.Max(baseSize.Height, unitHeight));
            }

            return new Size(baseWidth, baseSize.Height);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var textSize = arrangeBounds;
            var unit = EnsureUnitText(invalidate: false);
            var padding = this.UnitPadding;

            if (unit != null)
            {
                var unitWidth = unit.Width + padding.Left + padding.Right;
                var unitHeight = unit.Height + padding.Top + padding.Bottom;

                textSize.Width -= unitWidth;
                _unitBounds = new Rect(textSize.Width + padding.Left, (arrangeBounds.Height - unitHeight) / 2 + padding.Top, textSize.Width, textSize.Height);
            }

            var baseSize = base.ArrangeOverride(textSize);

            if (unit != null)
            {
                var unitWidth = unit.Width + padding.Left + padding.Right;
                var unitHeight = unit.Height + padding.Top + padding.Bottom;

                return new Size( baseSize.Width + unitWidth, Math.Max(baseSize.Height, unitHeight));
            }
            return baseSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var unitText = EnsureUnitText(false);
            if (unitText != null)
                drawingContext.DrawText(unitText, _unitBounds.Location);
        }

        private FormattedText EnsureUnitText(bool invalidate = false)
        {
            if (invalidate)
                _unit = null;

            if (_unit != null)
                return _unit;

            var unit = this.Unit;

            if (!string.IsNullOrEmpty(unit))
            {
                _unit = new FormattedText(unit, CultureInfo.InvariantCulture, this.FlowDirection, new Typeface( this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch), this.FontSize, this.Foreground);
            }

            return _unit;
        }

        //static UnitTextBoxControl()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(UnitTextBoxControl), new FrameworkPropertyMetadata(typeof(UnitTextBoxControl)));
        //}
        //public UnitTextBoxControl()
        //{
        //    TextChanged += new TextChangedEventHandler(MyTextChanged);
        //}

        //private void MyTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (Text == "")
        //        Text = Unit;

        //    else if (Text.IndexOf(Unit, StringComparison.Ordinal) == -1)
        //    {
        //        int tmp = ((TextBox)e.Source).SelectionStart;
        //        //Text += ' ' + Unit; // append unit and reset cursor position
        //        ((TextBox)e.Source).SelectionStart = tmp; // restore cursor
        //    }
        //}    
    }
}
