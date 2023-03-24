using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public static class Notch
    {
        [IIRFilterAttr(FilterType.Notch, FilterPassType.None, 1, 2)]
        public static IIRFilter Create(FilterParameters parameters)
        {
            if (parameters.BW == null)
                throw new ArgumentException("Bandwidth not specified");

            double bw = parameters.BW ?? 100;
            int fc = parameters.Fc;
            int fs = parameters.Fs;

            double alpha = Math.Tan(Math.PI * bw / fs);
            double beta = -Math.Cos(2 * Math.PI * fc / fs);
            double D = alpha + 1;

            double[] a = new double[2];
            double[] b = new double[3];
            b[0] = 1;
            b[1] = 2 * beta;
            b[2] = 1;
            a[0] = b[1];
            a[1] = 1 - alpha;
 
            for (int i = 0; i < a.Length; i++)
            {
                a[i] /= D;
            }

            for (int i = 0; i < b.Length; i++)
            {
                b[i] /= D;
            }

            return new IIRFilter(a, b, parameters);
        }
    }
}
