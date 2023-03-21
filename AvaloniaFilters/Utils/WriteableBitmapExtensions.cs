using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
