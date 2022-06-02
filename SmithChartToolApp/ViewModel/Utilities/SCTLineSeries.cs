using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartToolApp.ViewModel.Utilities
{
    /// <summary>
    /// Custom Line series to expand existing Oxyplot series
    /// </summary>
    public class SCTLineSeries : LineSeries
    {
        public bool Aliased { get; set; } = true;

        //List<ScreenPoint> outputBuffer = null;

        //protected override void RenderLine(IRenderContext rc, OxyRect clippingRect, IList<ScreenPoint> pointsToRender)
        //{
        //    var dashArray = this.ActualDashArray;

        //    if (this.outputBuffer == null)
        //    {
        //        this.outputBuffer = new List<ScreenPoint>(pointsToRender.Count);
        //    }

        //    rc.DrawClippedLine(clippingRect,
        //                       pointsToRender,
        //                       this.MinimumSegmentLength * this.MinimumSegmentLength,
        //                       this.GetSelectableColor(this.ActualColor),
        //                       this.StrokeThickness,
        //                       dashArray,
        //                       this.LineJoin,
        //                       this.Aliased,  // <-- this is the issue
        //                       this.outputBuffer);

        //}
    }
}
