﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaFilters
{
    // Lines to plot on the graph at Value + i * Interval,
    // as long as the distance between lines is more than MinPointsSpacing
    public class LinesDefinition
    {
        public float Value { get; private set; }
        public float Interval { get; private set; }
        public bool Solid { get; private set; }
        public uint Color { get; private set; }
        public int MinPointsSpacing { get; private set; }
        public LinesDefinition(float value, float interval, bool solid, uint color, int minPointSpacing = 10)
        {
            Value = value;
            Interval = interval;
            Solid = solid;
            Color = color;
            MinPointsSpacing = minPointSpacing;
        }
    }
}
