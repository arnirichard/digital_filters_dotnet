using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using AvaloniaFilters.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaFilters
{
    public enum ScaleType
    {
        Linear,
        Log2,
        Log10
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
        public LinesDefinition[]? YValueLines { get; set; }
        public LinesDefinition[]? XValueLines { get; set; }

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
            WriteableBitmap? wbitmap = await CreateBitmap(vm, width, height);

            if(wbitmap != null && 
                width == currentWidth &&
                height == currentHeight)
            {
                image.Source = wbitmap;
            }
        }

        async Task<WriteableBitmap?> CreateBitmap(PlotViewModel vm, int width, int height)
        {
            TaskCompletionSource<WriteableBitmap?> taskCompletionSource =
                new TaskCompletionSource<WriteableBitmap?>();

            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    WriteableBitmap writeableBitmap = new WriteableBitmap(
                        new PixelSize(width, height),
                        new Vector(96, 96),
                        Avalonia.Platform.PixelFormat.Bgra8888,
                        Avalonia.Platform.AlphaFormat.Unpremul);

                    //writeableBitmap.WriteColor(Beige);

                    taskCompletionSource.SetResult(writeableBitmap);
                }
                catch
                {
                    taskCompletionSource.SetResult(null);   
                }
            });

            return await taskCompletionSource.Task;
        }
    }
}
