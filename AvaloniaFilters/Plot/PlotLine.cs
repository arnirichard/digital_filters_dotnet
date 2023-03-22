using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaFilters
{
    // Line that as plotted on the grap
    public class PlotLine
    {
        public int Position { get; private set; }
        public double Value { get; private set; }
        public bool Solid { get; private set; }

        public PlotLine(int position, double value, bool solid)
        {
            Position = position;
            Value = value;
            Solid = solid;
        }
    }
}
