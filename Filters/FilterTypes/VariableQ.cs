using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    internal class VariableQ
    {
        [IIRFilterAttr(FilterType.VariableQ, FilterPassType.BandPass)]
        internal static IIRFilter BandPass(double Q, int f_c, int f_s, double bw)
        {
            throw new NotImplementedException();
        }

        [IIRFilterAttr(FilterType.VariableQ, FilterPassType.BandStop)]
        internal static IIRFilter BandStop(double Q, int f_c, int f_s, double bw)
        {
            throw new NotImplementedException();
        }

        [IIRFilterAttr(FilterType.VariableQ, FilterPassType.HighPass)]
        internal static IIRFilter HighPass(double Q, int f_c, int f_s)
        {
            throw new NotImplementedException();
        }

        [IIRFilterAttr(FilterType.VariableQ, FilterPassType.LowPass)]
        internal static IIRFilter LowPass(double Q, int f_c, int f_s)
        {
            throw new NotImplementedException();
        }
    }
}
