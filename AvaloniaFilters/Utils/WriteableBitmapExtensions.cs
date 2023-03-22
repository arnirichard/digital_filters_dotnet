using Avalonia;
using Avalonia.Input.Raw;
using Avalonia.Media.Imaging;
using AvaloniaFilters.Utils;
using DynamicData;
using Filters;
using JetBrains.Annotations;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaFilters
{
    public static class WriteableBitmapExtensions
    {
        public unsafe static void WriteColor(this WriteableBitmap bm, uint color)
        {
            int numPixels = bm.PixelSize.Width*bm.PixelSize.Height;

            using (var buf = bm.Lock())
            {
                var ptr = (uint*)buf.Address;

                for (int y = 0; y < numPixels; y++)
                { 
                    *ptr = color;
                    ptr++;
                }
            }
        }

        public unsafe static List<PlotLine> PlotHorizontalLines(this WriteableBitmap writeableBitmap, 
            NumberRange<double> range, int logBase, LinesDefinition[] linesDefinitions)
        {
            List<PlotLine> result = new();
            HashSet<int> points = new();

            foreach (var linesDefinition in linesDefinitions)
            {
                var spacing = writeableBitmap.PixelSize.Height *
                    (linesDefinition.Interval > 0 ? linesDefinition.Interval : range.Length)
                / range.Length;

                if (range.Length > 0 && spacing >= linesDefinition.MinPointsSpacing)
                {
                    double val = linesDefinition.Value - linesDefinition.Interval * (int)((linesDefinition.Value - range.Start) / linesDefinition.Interval);
                    int pos;

                    int lastPos = writeableBitmap.PixelSize.Height;

                    while (val < range.End)
                    {
                        if (val > range.Start)
                        {
                            var ratio = val.GetLogarithmicRatio(range.Start, range.End, logBase);
                            pos = (int)((1 - ratio) * writeableBitmap.PixelSize.Height);
                            if (!points.Contains(pos) &&
                                Math.Abs(lastPos - pos) > linesDefinition.MinPointsSpacing &&
                                    writeableBitmap.PlotHorizontalLine(linesDefinition.Color,
                                        pos,
                                        linesDefinition.Solid ? writeableBitmap.PixelSize.Width : 8,
                                        linesDefinition.Solid ? 0 : 4))
                            {   
                                lastPos = pos;
                                result.Add(new PlotLine(pos, val, linesDefinition.Solid));
                                points.Add(pos);
                            }
                        }

                        val += linesDefinition.Interval <= 0 ? int.MaxValue : linesDefinition.Interval;
                    }
                }
            }

            return result;
        }

        internal unsafe static bool PlotHorizontalLine(this WriteableBitmap bm, uint color, int y,
            int solid = int.MaxValue, int gaps = 0)
        {
            if (y < 0 || y >= bm.PixelSize.Height)
                return false;

            using (var buf = bm.Lock())
            {
                var ptr = (uint*)buf.Address;

                ptr += y * bm.PixelSize.Width;
                int width = bm.PixelSize.Width;

                int i = 0;
                int iTo;

                while (true)
                {
                    iTo = Math.Min(i + solid, width);

                    for (; i < iTo; i++)
                    {
                        *ptr = color;
                        ptr ++;
                    }

                    if (iTo < width && gaps > 0)
                    {
                        ptr += gaps;
                        i += gaps;
                    }

                    if (i >= width)
                        break;
                }
            }

            return true;
        }

        public unsafe static List<PlotLine> PlotVerticallLines(this WriteableBitmap writeableBitmap,
            NumberRange<double> range, int logBase, LinesDefinition[] linesDefinitions)
        {
            List<PlotLine> result = new();
            HashSet<int> points = new();

            foreach (var linesDefinition in linesDefinitions)
            {
                var spacing = writeableBitmap.PixelSize.Width *
                    (linesDefinition.Interval > 0 ? linesDefinition.Interval : range.Length)
                    / range.Length;

                if (range.Length > 0 && spacing >= linesDefinition.MinPointsSpacing)
                {
                    double val = linesDefinition.Value + linesDefinition.Interval * (int)((linesDefinition.Value - range.Start) / linesDefinition.Interval);
                    int pos;

                    while (val < range.End)
                    {
                        if (val > range.Start)
                        {
                            var ratio = val.GetLogarithmicRatio(range.Start, range.End, logBase);
                            pos = (int)(ratio * writeableBitmap.PixelSize.Width);

                            if (!points.Contains(pos) &&
                                writeableBitmap.PaintVerticalLine(linesDefinition.Color,
                                    pos,
                                    linesDefinition.Solid ? writeableBitmap.PixelSize.Height : 8,
                                    linesDefinition.Solid ? 0 : 4))
                            {
                                result.Add(new PlotLine(pos, val, linesDefinition.Solid));
                                points.Add(pos);
                            }
                        }

                        if (linesDefinition.Interval <= 0)
                            break;

                        val += linesDefinition.Interval;
                    }
                }
            }


            return result;
        }

        internal unsafe static bool PaintVerticalLine(this WriteableBitmap writeableBitmap, uint color, int x,
            int solid = int.MaxValue, int gaps = 0)
        {
            if (x < 0 || x >= writeableBitmap.PixelSize.Width)
                return false;

            using (var buf = writeableBitmap.Lock())
            {
                var ptr = (uint*)buf.Address;

                ptr += x;
                int height = writeableBitmap.PixelSize.Height;

                int i = 0;
                int iTo;

                while (true)
                {
                    iTo = Math.Min(i + solid, height);

                    for (; i < iTo; i++)
                    {
                        *ptr = color;
                        ptr += writeableBitmap.PixelSize.Width;
                    }

                    if (iTo < height && gaps > 0)
                    {
                        ptr += writeableBitmap.PixelSize.Width * gaps;
                        i += gaps;
                    }

                    if (i >= height)
                        break;
                }
            }
            
            return true;
        }

        public unsafe static void PlotXY(this WriteableBitmap bm, uint color,
            AxisData yData, AxisData xData)
        {
            if (yData.Values.Length == 0)
                return;

            double[] xPlotRange = xData.VisibleRange.Start.GetLogarithmicRange(xData.VisibleRange.End, bm.PixelSize.Width, xData.LogBase);

            int index = 0;
            double xVal = xData.Values[index];
            double currentX, ratio;
            int py;
            bool isInsidePlot;
            int lastPxInsidePlot = -1;
            double lastYInsidePlot = 0;
            int pixelShift;

            using (var buf = bm.Lock())
            {
                var ptr = (uint*)buf.Address;

                for (int px = 0; px < bm.PixelSize.Width; px++)
                {
                    currentX = xPlotRange[px];
                    if (xVal <= currentX)
                    {
                        while (index < xData.Values.Length - 1 && xData.Values[index + 1] <= currentX)
                        {
                            index++;
                            xVal = xData.Values[index];
                            continue;
                        }

                        ratio = yData.Values[index].GetLogarithmicRatio(yData.VisibleRange.Start, yData.VisibleRange.End, yData.LogBase);
                        py = (int)((1 - ratio) * bm.PixelSize.Height);
                        isInsidePlot = py >= 0 && py < bm.PixelSize.Height;

                        if (isInsidePlot)
                        {
                            pixelShift = px + py * bm.PixelSize.Width;
                            * (ptr + pixelShift) = color;
                            if(py > 0)
                                *(ptr + pixelShift - bm.PixelSize.Width) = color;
                        }

                        if (px - lastPxInsidePlot > 1)
                        {
                            var yrange = lastYInsidePlot.GetLogarithmicRange(yData.Values[index], px-lastPxInsidePlot+1, yData.LogBase);
                            for (int px2 = 1; px2 < yrange.Length - 1; px2++)
                            {
                                ratio = yrange[px2].GetLogarithmicRatio(yData.VisibleRange.Start, yData.VisibleRange.End, yData.LogBase);
                                py = (int)((1 - ratio) * bm.PixelSize.Height);
                                if (py < 0 || py >= bm.PixelSize.Height)
                                {
                                    break;
                                }
                                pixelShift = lastPxInsidePlot + px2 + py * bm.PixelSize.Width;
                                * (ptr + pixelShift) = color;
                                if (py > 0)
                                    *(ptr + pixelShift - bm.PixelSize.Width) = color;
                            }
                        }

                        lastPxInsidePlot = isInsidePlot ? px : bm.PixelSize.Width;

                        if (isInsidePlot)
                            lastYInsidePlot = yData.Values[index];

                        index++;
                        if (index >= xData.Values.Length)
                            break;

                        xVal = xData.Values[index];
                    }
                }
            }
        }
    }
}
