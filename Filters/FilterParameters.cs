using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public class FilterParameters
    {
        public required int Fs { get; init; }
        public required int Fc { get; init; }
        public int? Order;
        public double? BW;
        public double? Q;
        public double? LinearGain;
        public double? RippleFactor;

        public FilterParameters(int? order = null, double? bW = null,
            double? q = null, double? linearGain = null)
        {
            Order = order;
            BW = bW;
            Q = q;
            LinearGain = linearGain;
        }
    }
}
