using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public class FilterParameters
    {
        public int? Order;
        public int Fs;
        public int Fc;
        public double? BW;
        public double? Q;
        public double? LinearGain;

        public FilterParameters(int? order = null, int fs = 0, int fc = 0, double? bW = null,
            double? q = null, double? linearGain = null)
        {
            Order = order;
            Fs = fs;
            Fc = fc;
            BW = bW;
            Q = q;
            LinearGain = linearGain;
        }
    }
}
