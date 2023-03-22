using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaFilters.Utils;
using Filters;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaFilters
{
    public enum ScaleType
    {
        Linear = 1,
        Log2 = 2,
        Log10 = 10
    }

    public partial class Plot : UserControl
    {
        public static uint Red = uint.Parse("FFFF0000", System.Globalization.NumberStyles.HexNumber);
        public static uint Orange = uint.Parse("FFFF6A00", System.Globalization.NumberStyles.HexNumber);
        public static uint Black = uint.Parse("FF000000", System.Globalization.NumberStyles.HexNumber);
        public static uint White = uint.Parse("FFFFFFFF", System.Globalization.NumberStyles.HexNumber);
        public static uint Beige = uint.Parse("FFDDDDDD", System.Globalization.NumberStyles.HexNumber);
        public static uint Blue = uint.Parse("FF0000FF", System.Globalization.NumberStyles.HexNumber);
        public ScaleType YScaleType { get; set; } = ScaleType.Linear;
        public ScaleType XScaleType { get; set; } = ScaleType.Linear;
        public NumberRange<double>? YDisplayRange { get; set; }
        public NumberRange<double>? XDisplayRange { get; set; }
        public LinesDefinition[]? HorizontalLines { get; set; }
        public LinesDefinition[]? VerticalLines { get; set; }

        int currentWidth, currentHeight;

        public Plot()
        {
            InitializeComponent();

            DataContextChanged += GridPlot_DataContextChanged;

            grid.GetObservable(BoundsProperty).Subscribe(value =>
            {
                if (DataContext is PlotViewModel vm)
                {
                    _ = Redraw(vm);
                }
            });
        }

        private void GridPlot_DataContextChanged(object? sender, EventArgs e)
        {
            if (DataContext is PlotViewModel vm)
            {
                _ = Redraw(vm);
            }
        }

        async Task Redraw(PlotViewModel vm)
        {
            if (grid.Bounds.Width == 0 || grid.Bounds.Height == 0)
                return;

            int width = currentWidth = (int)grid.Bounds.Width;
            int height = currentHeight = (int)grid.Bounds.Height;
            DrawBitmapJob? wbitmap = await CreateBitmap(vm, width, height);

            if(wbitmap != null && 
                width == currentWidth &&
                height == currentHeight)
            {
                image.Source = wbitmap.Bitmap;
                AddLabels(belowLabels, wbitmap.VerticalLines, true);
                AddLabels(sideLabels, wbitmap.HorizontalLines, false);
            }
        }

        void AddLabels(Canvas canvas, List<PlotLine> plotLines, bool below)
        {
            canvas.Children.Clear();

            foreach (var plotLine in plotLines)
            {
                TextBlock textBlock = new TextBlock()
                {
                    Text = plotLine.Value.ToString("0.###"),
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Black)
                };
                if (below)
                {
                    textBlock.Width = 100;
                    Canvas.SetLeft(textBlock, plotLine.Position - 50);
                    Canvas.SetTop(textBlock, 3);
                }
                else
                {
                    Canvas.SetRight(textBlock, 5);
                    Canvas.SetTop(textBlock, plotLine.Position - 8);
                }
                canvas.Children.Add(textBlock);
            }
        }

        class DrawBitmapJob
        {
            public WriteableBitmap Bitmap;
            public List<PlotLine> HorizontalLines;
            public List<PlotLine> VerticalLines;
        }

        async Task<DrawBitmapJob?> CreateBitmap(PlotViewModel vm, int width, int height)
        {
            TaskCompletionSource<DrawBitmapJob?> taskCompletionSource =
                new TaskCompletionSource<DrawBitmapJob?>();

            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    WriteableBitmap writeableBitmap = new WriteableBitmap(
                        new PixelSize(width, height),
                        new Vector(96, 96),
                        Avalonia.Platform.PixelFormat.Bgra8888,
                        Avalonia.Platform.AlphaFormat.Unpremul);

                    NumberRange<double> xRange = vm.XRange ?? new NumberRange<double>(1, vm.Y.Length);
                    double[] x = vm.X ?? 1D.GetLinearRange(vm.Y.Length, vm.Y.Length);

                    AxisData yAxisData = new AxisData(vm.Y, vm.YRange, vm.YRange, (int)YScaleType);
                    AxisData xAxisData = new AxisData(x, xRange, xRange, (int)XScaleType);

                    List<PlotLine> horizontalPlotLines = new();
                    List<PlotLine> verticalPlotLines = new();

                    if (HorizontalLines != null)
                        horizontalPlotLines = writeableBitmap.PlotHorizontalLines(yAxisData.VisibleRange, (int)YScaleType, HorizontalLines);

                    if(VerticalLines != null)
                        verticalPlotLines = writeableBitmap.PlotVerticallLines(xAxisData.VisibleRange, (int)XScaleType, VerticalLines);

                    writeableBitmap.PlotXY(Black, yAxisData, xAxisData);

                    taskCompletionSource.SetResult(new DrawBitmapJob()
                    {
                        Bitmap = writeableBitmap,
                        HorizontalLines = horizontalPlotLines,
                        VerticalLines = verticalPlotLines
                    });
                }
                catch(Exception ex)
                {
                    taskCompletionSource.SetResult(null);   
                }
            });

            return await taskCompletionSource.Task;
        }
    }
}
