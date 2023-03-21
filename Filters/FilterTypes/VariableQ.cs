using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    internal class VariableQ
    {
        internal static IIRFilter BandPass(double Q, int f_c, int f_s, double bw)
        {
            throw new NotImplementedException();
        }

        internal static IIRFilter BandStop(double Q, int f_c, int f_s, double bw)
        {
            throw new NotImplementedException();
        }

        internal static IIRFilter HighPass(double Q, int f_c, int f_s)
        {
            throw new NotImplementedException();
        }

        internal static IIRFilter LowPass(double Q, int f_c, int f_s)
        {
            throw new NotImplementedException();
        }
    }
}
